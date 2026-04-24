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
    public class ReviewAutoSchedulingService : IReviewAutoSchedulingService
    {
        private readonly IProjectGroupAutoScheduleRepository _projectGroupRepository;
        private readonly IReviewRoundAutoScheduleRepository _reviewRoundRepository;
        private readonly ITeacherAutoScheduleRepository _teacherRepository;
        private readonly ITeacherAvailabilityAutoScheduleRepository _teacherAvailabilityRepository;
        private readonly ITimeSlotAutoScheduleRepository _timeSlotRepository;
        private readonly IReviewAssignmentAutoScheduleRepository _reviewAssignmentRepository;
        private readonly IReviewAssignmentTeacherAutoScheduleRepository _reviewAssignmentTeacherRepository;

        public ReviewAutoSchedulingService(
            IProjectGroupAutoScheduleRepository projectGroupRepository,
            IReviewRoundAutoScheduleRepository reviewRoundRepository,
            ITeacherAutoScheduleRepository teacherRepository,
            ITeacherAvailabilityAutoScheduleRepository teacherAvailabilityRepository,
            ITimeSlotAutoScheduleRepository timeSlotRepository,
            IReviewAssignmentAutoScheduleRepository reviewAssignmentRepository,
            IReviewAssignmentTeacherAutoScheduleRepository reviewAssignmentTeacherRepository)
        {
            _projectGroupRepository = projectGroupRepository;
            _reviewRoundRepository = reviewRoundRepository;
            _teacherRepository = teacherRepository;
            _teacherAvailabilityRepository = teacherAvailabilityRepository;
            _timeSlotRepository = timeSlotRepository;
            _reviewAssignmentRepository = reviewAssignmentRepository;
            _reviewAssignmentTeacherRepository = reviewAssignmentTeacherRepository;
        }

        public async Task<(bool Success, string Message, AutoScheduleBySemesterResultDto? Data)> AutoScheduleBySemesterAsync(
            long assignedByUserId,
            AutoScheduleBySemesterRequestDto request)
        {
            if (request.SemesterId <= 0)
                return (false, "SemesterId không hợp lệ.", null);

            var groups = await _projectGroupRepository.GetBySemesterAsync(request.SemesterId);

            var result = new AutoScheduleBySemesterResultDto
            {
                TotalGroups = groups.Count
            };

            if (!groups.Any())
                return (true, "Không có nhóm nào trong học kỳ này để xếp lịch.", result);

            var semester = groups.First().ProjectCourse.Semester;
            var fromDate = semester.StartDate;
            var toDate = semester.EndDate;

            const int roundNumber = 1;
            const string roundName = "Round 1";
            const int teachersPerAssignment = 2;
            const string assignmentStatus = "OPEN";

            var timeSlots = await _timeSlotRepository.GetActiveAsync();
            if (!timeSlots.Any())
                return (false, "Không có TimeSlot nào đang active để xếp lịch.", null);

            var teachers = await _teacherRepository.GetAvailableForReviewAsync();
            if (!teachers.Any())
                return (false, "Không có giảng viên nào sẵn sàng tham gia chấm.", null);

            var allAvailabilities = await _teacherAvailabilityRepository.GetAvailableInRangeAsync(
                fromDate,
                toDate,
                timeSlots.Select(x => x.TimeSlotId).ToList());

            var availabilitySet = allAvailabilities
                .Select(x => BuildSlotKey(x.TeacherId, x.AvailableDate, x.TimeSlotId))
                .ToHashSet();

            var existingAssignments = await _reviewAssignmentTeacherRepository.GetBySemesterAsync(request.SemesterId);

            var sameSlotAssignedSet = existingAssignments
                .Select(x => BuildSlotKey(
                    x.TeacherId,
                    x.ReviewAssignment.AssignedDate,
                    x.ReviewAssignment.TimeSlotId))
                .ToHashSet();

            var dayCountMap = existingAssignments
                .GroupBy(x => BuildDayKey(x.TeacherId, x.ReviewAssignment.AssignedDate))
                .ToDictionary(g => g.Key, g => g.Count());

            var totalLoadMap = existingAssignments
                .GroupBy(x => x.TeacherId)
                .ToDictionary(g => g.Key, g => g.Count());

            var allDates = GetDates(fromDate, toDate);

            foreach (var group in groups)
            {
                var reviewRound = await GetOrCreateReviewRoundAsync(group, roundNumber, roundName);

                if (reviewRound.ReviewAssignment != null)
                {
                    result.FailedItems.Add(new AutoScheduleBySemesterFailedItemDto
                    {
                        ReviewRoundId = reviewRound.ReviewRoundId,
                        GroupCode = group.GroupCode,
                        GroupName = group.GroupName,
                        Reason = "Nhóm này đã có lịch chấm cho Round 1."
                    });
                    continue;
                }

                var supervisorIds = group.ProjectSupervisors
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
                                availabilitySet.Contains(BuildSlotKey(t.TeacherId, date, slot.TimeSlotId)) &&
                                !sameSlotAssignedSet.Contains(BuildSlotKey(t.TeacherId, date, slot.TimeSlotId)) &&
                                dayCountMap.GetValueOrDefault(BuildDayKey(t.TeacherId, date), 0) < t.MaxAssignmentsPerDay)
                            .OrderBy(t => totalLoadMap.GetValueOrDefault(t.TeacherId, 0))
                            .ThenBy(t => dayCountMap.GetValueOrDefault(BuildDayKey(t.TeacherId, date), 0))
                            .ThenBy(t => t.TeacherCode)
                            .Take(teachersPerAssignment)
                            .ToList();

                        if (candidateTeachers.Count < teachersPerAssignment)
                            continue;

                        var assignment = new ReviewAssignment
                        {
                            ReviewRoundId = reviewRound.ReviewRoundId,
                            AssignedByUserId = assignedByUserId,
                            AssignedDate = date,
                            TimeSlotId = slot.TimeSlotId,
                            Location = null,
                            Status = assignmentStatus,
                            Note = null
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

                        result.ScheduledItems.Add(new AutoScheduleBySemesterScheduledItemDto
                        {
                            ReviewAssignmentId = created.ReviewAssignmentId,
                            ReviewRoundId = reviewRound.ReviewRoundId,
                            GroupCode = group.GroupCode,
                            GroupName = group.GroupName,
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
                    result.FailedItems.Add(new AutoScheduleBySemesterFailedItemDto
                    {
                        ReviewRoundId = reviewRound.ReviewRoundId,
                        GroupCode = group.GroupCode,
                        GroupName = group.GroupName,
                        Reason = "Không tìm được ngày/slot có đủ giảng viên hợp lệ."
                    });
                }
            }

            result.ScheduledCount = result.ScheduledItems.Count;
            result.FailedCount = result.FailedItems.Count;

            return (true, "Tự động xếp lịch hoàn tất.", result);
        }

        private async Task<ReviewRound> GetOrCreateReviewRoundAsync(
            ProjectGroup projectGroup,
            int roundNumber,
            string roundName)
        {
            var existing = await _reviewRoundRepository
                .GetByProjectGroupAndRoundNumberAsync(projectGroup.ProjectGroupId, roundNumber);

            if (existing != null)
                return existing;

            return await _reviewRoundRepository.CreateAsync(new ReviewRound
            {
                ProjectGroupId = projectGroup.ProjectGroupId,
                RoundNumber = roundNumber,
                RoundName = roundName,
                Status = "PENDING"
            });
        }

        private static List<DateOnly> GetDates(DateOnly fromDate, DateOnly toDate)
        {
            var dates = new List<DateOnly>();
            for (var d = fromDate; d <= toDate; d = d.AddDays(1))
            {
                dates.Add(d);
            }
            return dates;
        }

        private static string BuildSlotKey(long teacherId, DateOnly date, long timeSlotId)
            => $"{teacherId}_{date:yyyy-MM-dd}_{timeSlotId}";

        private static string BuildDayKey(long teacherId, DateOnly date)
            => $"{teacherId}_{date:yyyy-MM-dd}";

        private static string BuildPanelRole(int index, int total)
        {
            if (total == 1)
                return "REVIEWER";

            return $"REVIEWER_{index + 1}";
        }
    }
}
