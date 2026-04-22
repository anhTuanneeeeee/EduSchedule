using Schedule_Repository.IRepository;
using Schedule_Repository.Models;
using Schedule_Repository.Repository;
using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.Service
{
    public class ReviewSchedulingService : IReviewSchedulingService
    {
        private readonly IReviewRoundRepository _reviewRoundRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ITeacherAvailabilityRepository _teacherAvailabilityRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IReviewAssignmentRepository _reviewAssignmentRepository;
        private readonly IReviewAssignmentTeacherRepository _reviewAssignmentTeacherRepository;

        public ReviewSchedulingService(
            IReviewRoundRepository reviewRoundRepository,
            ITeacherRepository teacherRepository,
            ITeacherAvailabilityRepository teacherAvailabilityRepository,
            ITimeSlotRepository timeSlotRepository,
            IReviewAssignmentRepository reviewAssignmentRepository,
            IReviewAssignmentTeacherRepository reviewAssignmentTeacherRepository)
        {
            _reviewRoundRepository = reviewRoundRepository;
            _teacherRepository = teacherRepository;
            _teacherAvailabilityRepository = teacherAvailabilityRepository;
            _timeSlotRepository = timeSlotRepository;
            _reviewAssignmentRepository = reviewAssignmentRepository;
            _reviewAssignmentTeacherRepository = reviewAssignmentTeacherRepository;
        }

        public async Task<(bool Success, string Message, AutoScheduleResultDto? Data)> AutoScheduleAsync(
            long assignedByUserId,
            AutoScheduleReviewAssignmentsRequestDto request)
        {
            if (request.SemesterId <= 0)
                return (false, "SemesterId không hợp lệ.", null);

            if (request.FromDate > request.ToDate)
                return (false, "FromDate phải nhỏ hơn hoặc bằng ToDate.", null);

            if (request.TeachersPerAssignment <= 0)
                return (false, "TeachersPerAssignment phải lớn hơn 0.", null);

            var normalizedStatus = NormalizeAssignmentStatus(request.Status);
            if (normalizedStatus == null)
                return (false, "Status chỉ chấp nhận DRAFT, OPEN hoặc CONFIRMED.", null);

            var timeSlots = await _timeSlotRepository.GetActiveAsync(request.TimeSlotIds);
            if (!timeSlots.Any())
                return (false, "Không có TimeSlot hợp lệ để xếp lịch.", null);

            var reviewRounds = await _reviewRoundRepository.GetUnscheduledBySemesterAsync(request.SemesterId);
            var teachers = await _teacherRepository.GetAvailableForReviewAsync();

            var result = new AutoScheduleResultDto
            {
                TotalReviewRounds = reviewRounds.Count
            };

            if (!reviewRounds.Any())
                return (true, "Không có ReviewRound nào cần xếp lịch.", result);

            if (!teachers.Any())
                return (false, "Không có giảng viên nào sẵn sàng tham gia chấm.", null);

            var allAvailabilities = await _teacherAvailabilityRepository.GetAvailableInRangeAsync(
                request.FromDate,
                request.ToDate,
                timeSlots.Select(x => x.TimeSlotId).ToList());

            var availabilityMap = allAvailabilities
                .GroupBy(x => BuildSlotKey(x.TeacherId, x.AvailableDate, x.TimeSlotId))
                .ToDictionary(x => x.Key, x => true);

            var existingAssignments = await _reviewAssignmentTeacherRepository.GetBySemesterAsync(request.SemesterId);

            var sameSlotAssignedSet = existingAssignments
                .Select(x => BuildSlotKey(x.TeacherId, x.ReviewAssignment.AssignedDate, x.ReviewAssignment.TimeSlotId))
                .ToHashSet();

            var dayCountMap = existingAssignments
                .GroupBy(x => BuildDayKey(x.TeacherId, x.ReviewAssignment.AssignedDate))
                .ToDictionary(g => g.Key, g => g.Count());

            var totalLoadMap = existingAssignments
                .GroupBy(x => x.TeacherId)
                .ToDictionary(g => g.Key, g => g.Count());

            var allDates = GetDates(request.FromDate, request.ToDate);

            foreach (var round in reviewRounds)
            {
                var supervisorIds = round.ProjectGroup.ProjectSupervisors
                    .Select(x => x.TeacherId)
                    .ToHashSet();

                bool scheduled = false;

                foreach (var date in allDates)
                {
                    foreach (var slot in timeSlots)
                    {
                        var candidateTeachers = teachers
                            .Where(t =>
                                !supervisorIds.Contains(t.TeacherId) &&
                                availabilityMap.ContainsKey(BuildSlotKey(t.TeacherId, date, slot.TimeSlotId)) &&
                                !sameSlotAssignedSet.Contains(BuildSlotKey(t.TeacherId, date, slot.TimeSlotId)) &&
                                dayCountMap.GetValueOrDefault(BuildDayKey(t.TeacherId, date), 0) < t.MaxAssignmentsPerDay)
                            .OrderBy(t => totalLoadMap.GetValueOrDefault(t.TeacherId, 0))
                            .ThenBy(t => dayCountMap.GetValueOrDefault(BuildDayKey(t.TeacherId, date), 0))
                            .ThenBy(t => t.TeacherCode)
                            .Take(request.TeachersPerAssignment)
                            .ToList();

                        if (candidateTeachers.Count < request.TeachersPerAssignment)
                            continue;

                        var assignment = new ReviewAssignment
                        {
                            ReviewRoundId = round.ReviewRoundId,
                            AssignedByUserId = assignedByUserId,
                            AssignedDate = date,
                            TimeSlotId = slot.TimeSlotId,
                            Location = request.DefaultLocation,
                            Status = normalizedStatus,
                            Note = request.Note
                        };

                        var teacherLinks = candidateTeachers
                            .Select((teacher, index) => new ReviewAssignmentTeacher
                            {
                                TeacherId = teacher.TeacherId,
                                RoleInPanel = BuildPanelRole(index, candidateTeachers.Count)
                            })
                            .ToList();

                        var created = await _reviewAssignmentRepository.CreateWithTeachersAsync(assignment, teacherLinks);

                        foreach (var teacher in candidateTeachers)
                        {
                            sameSlotAssignedSet.Add(BuildSlotKey(teacher.TeacherId, date, slot.TimeSlotId));

                            var dayKey = BuildDayKey(teacher.TeacherId, date);
                            dayCountMap[dayKey] = dayCountMap.GetValueOrDefault(dayKey, 0) + 1;

                            totalLoadMap[teacher.TeacherId] = totalLoadMap.GetValueOrDefault(teacher.TeacherId, 0) + 1;
                        }

                        result.ScheduledItems.Add(new AutoScheduledItemDto
                        {
                            ReviewAssignmentId = created.ReviewAssignmentId,
                            ReviewRoundId = round.ReviewRoundId,
                            GroupCode = round.ProjectGroup.GroupCode,
                            GroupName = round.ProjectGroup.GroupName,
                            AssignedDate = date,
                            TimeSlotId = slot.TimeSlotId,
                            SlotName = slot.SlotName,
                            TeacherCodes = candidateTeachers.Select(x => x.TeacherCode).ToList()
                        });

                        scheduled = true;
                        break;
                    }

                    if (scheduled)
                        break;
                }

                if (!scheduled)
                {
                    result.FailedItems.Add(new AutoScheduleFailedItemDto
                    {
                        ReviewRoundId = round.ReviewRoundId,
                        GroupCode = round.ProjectGroup.GroupCode,
                        GroupName = round.ProjectGroup.GroupName,
                        Reason = "Không tìm được ngày/slot có đủ giảng viên hợp lệ."
                    });
                }
            }

            result.ScheduledCount = result.ScheduledItems.Count;
            result.FailedCount = result.FailedItems.Count;

            return (true, "Tự động xếp lịch hoàn tất.", result);
        }

        public async Task<(bool Success, string Message, CreatedReviewAssignmentResponseDto? Data)> ManualScheduleAsync(
            long assignedByUserId,
            ManualScheduleReviewAssignmentRequestDto request)
        {
            if (request.ReviewRoundId <= 0)
                return (false, "ReviewRoundId không hợp lệ.", null);

            if (request.TimeSlotId <= 0)
                return (false, "TimeSlotId không hợp lệ.", null);

            var distinctTeacherIds = request.TeacherIds
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            if (!distinctTeacherIds.Any())
                return (false, "Bạn phải chọn ít nhất 1 giảng viên.", null);

            var normalizedStatus = NormalizeAssignmentStatus(request.Status);
            if (normalizedStatus == null)
                return (false, "Status chỉ chấp nhận DRAFT, OPEN hoặc CONFIRMED.", null);

            var reviewRound = await _reviewRoundRepository.GetByIdWithDetailsAsync(request.ReviewRoundId);
            if (reviewRound == null)
                return (false, "Không tìm thấy ReviewRound.", null);

            if (reviewRound.ReviewAssignment != null)
                return (false, "ReviewRound này đã được xếp lịch rồi.", null);

            var timeSlot = await _timeSlotRepository.GetByIdAsync(request.TimeSlotId);
            if (timeSlot == null || !timeSlot.IsActive)
                return (false, "TimeSlot không tồn tại hoặc đang bị khóa.", null);

            var semester = reviewRound.ProjectGroup.ProjectCourse.Semester;
            if (request.AssignedDate < semester.StartDate || request.AssignedDate > semester.EndDate)
                return (false, "AssignedDate phải nằm trong thời gian của học kỳ.", null);

            var teachers = await _teacherRepository.GetByIdsAsync(distinctTeacherIds);
            if (teachers.Count != distinctTeacherIds.Count)
                return (false, "Có TeacherId không tồn tại.", null);

            var availableSet = (await _teacherAvailabilityRepository.GetAvailableInRangeAsync(
                request.AssignedDate,
                request.AssignedDate,
                new List<long> { request.TimeSlotId }))
                .Select(x => x.TeacherId)
                .ToHashSet();

            var supervisorIds = reviewRound.ProjectGroup.ProjectSupervisors
                .Select(x => x.TeacherId)
                .ToHashSet();

            foreach (var teacher in teachers)
            {
                if (!teacher.IsAvailableForProjectReview || !teacher.User.IsActive)
                    return (false, $"Giảng viên {teacher.TeacherCode} hiện không được phép tham gia chấm.", null);

                if (supervisorIds.Contains(teacher.TeacherId))
                    return (false, $"Giảng viên {teacher.TeacherCode} đang hướng dẫn nhóm này nên không được chấm.", null);

                if (!availableSet.Contains(teacher.TeacherId))
                    return (false, $"Giảng viên {teacher.TeacherCode} chưa khai báo rảnh cho ngày/slot này.", null);

                var hasConflict = await _reviewAssignmentTeacherRepository.HasTeacherConflictSameSlotAsync(
                    teacher.TeacherId,
                    request.AssignedDate,
                    request.TimeSlotId);

                if (hasConflict)
                    return (false, $"Giảng viên {teacher.TeacherCode} đã có lịch khác trùng ngày và slot.", null);

                var dayCount = await _reviewAssignmentTeacherRepository.CountTeacherAssignmentsOnDateAsync(
                    teacher.TeacherId,
                    request.AssignedDate);

                if (dayCount >= teacher.MaxAssignmentsPerDay)
                    return (false, $"Giảng viên {teacher.TeacherCode} đã vượt MaxAssignmentsPerDay.", null);
            }

            var assignment = new ReviewAssignment
            {
                ReviewRoundId = request.ReviewRoundId,
                AssignedByUserId = assignedByUserId,
                AssignedDate = request.AssignedDate,
                TimeSlotId = request.TimeSlotId,
                Location = request.Location,
                Status = normalizedStatus,
                Note = request.Note
            };

            var teacherLinks = teachers
                .Select((teacher, index) => new ReviewAssignmentTeacher
                {
                    TeacherId = teacher.TeacherId,
                    RoleInPanel = BuildPanelRole(index, teachers.Count)
                })
                .ToList();

            var created = await _reviewAssignmentRepository.CreateWithTeachersAsync(assignment, teacherLinks);

            var response = new CreatedReviewAssignmentResponseDto
            {
                ReviewAssignmentId = created.ReviewAssignmentId,
                ReviewRoundId = reviewRound.ReviewRoundId,
                GroupCode = reviewRound.ProjectGroup.GroupCode,
                GroupName = reviewRound.ProjectGroup.GroupName,
                AssignedDate = created.AssignedDate,
                TimeSlotId = created.TimeSlotId,
                SlotName = timeSlot.SlotName,
                Status = created.Status,
                Location = created.Location,
                TeacherCodes = teachers.Select(x => x.TeacherCode).ToList()
            };

            return (true, "Xếp lịch thủ công thành công.", response);
        }

        private static List<DateOnly> GetDates(DateOnly fromDate, DateOnly toDate)
        {
            var result = new List<DateOnly>();
            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(date);
            }
            return result;
        }

        private static string BuildSlotKey(long teacherId, DateOnly date, long timeSlotId)
            => $"{teacherId}_{date:yyyy-MM-dd}_{timeSlotId}";

        private static string BuildDayKey(long teacherId, DateOnly date)
            => $"{teacherId}_{date:yyyy-MM-dd}";

        private static string? NormalizeAssignmentStatus(string? status)
        {
            var normalized = status?.Trim().ToUpperInvariant();
            return normalized is "DRAFT" or "OPEN" or "CONFIRMED"
                ? normalized
                : null;
        }

        private static string BuildPanelRole(int index, int total)
        {
            if (total == 1)
                return "REVIEWER";

            return $"REVIEWER_{index + 1}";
        }
    }
}
