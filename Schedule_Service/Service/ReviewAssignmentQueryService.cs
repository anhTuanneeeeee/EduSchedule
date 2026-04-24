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
    public class ReviewAssignmentQueryService : IReviewAssignmentQueryService
    {
        private readonly IReviewAssignmentQueryRepository _reviewAssignmentQueryRepository;
        private readonly IUserRepository _userRepository;

        public ReviewAssignmentQueryService(
            IReviewAssignmentQueryRepository reviewAssignmentQueryRepository,
            IUserRepository userRepository)
        {
            _reviewAssignmentQueryRepository = reviewAssignmentQueryRepository;
            _userRepository = userRepository;
        }

        public async Task<(bool Success, string Message, List<ScheduleOverviewDateDto>? Data)> GetScheduleOverviewAsync(
            long semesterId,
            DateOnly? fromDate = null,
            DateOnly? toDate = null,
            string? status = null)
        {
            if (semesterId <= 0)
                return (false, "SemesterId không hợp lệ.", null);

            if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
                return (false, "fromDate phải nhỏ hơn hoặc bằng toDate.", null);

            var assignments = await _reviewAssignmentQueryRepository.GetScheduleOverviewAsync(
                semesterId,
                fromDate,
                toDate,
                status);

            var result = assignments
                .GroupBy(x => x.AssignedDate)
                .Select(dateGroup => new ScheduleOverviewDateDto
                {
                    AssignedDate = dateGroup.Key,
                    Slots = dateGroup
                        .GroupBy(x => new
                        {
                            x.TimeSlotId,
                            x.TimeSlot.SlotNumber,
                            x.TimeSlot.SlotName,
                            x.TimeSlot.StartTime,
                            x.TimeSlot.EndTime
                        })
                        .Select(slotGroup => new ScheduleOverviewSlotDto
                        {
                            TimeSlotId = slotGroup.Key.TimeSlotId,
                            SlotNumber = slotGroup.Key.SlotNumber,
                            SlotName = slotGroup.Key.SlotName,
                            StartTime = slotGroup.Key.StartTime,
                            EndTime = slotGroup.Key.EndTime,
                            Assignments = slotGroup
                                .Select(MapAssignmentDto)
                                .OrderBy(x => x.GroupCode)
                                .ToList()
                        })
                        .OrderBy(x => x.SlotNumber)
                        .ToList()
                })
                .OrderBy(x => x.AssignedDate)
                .ToList();

            return (true, "Lấy danh sách lịch chấm thành công.", result);
        }

        private static ScheduleOverviewAssignmentDto MapAssignmentDto(ReviewAssignment item)
        {
            return new ScheduleOverviewAssignmentDto
            {
                ReviewAssignmentId = item.ReviewAssignmentId,
                ReviewRoundId = item.ReviewRoundId,
                RoundNumber = item.ReviewRound.RoundNumber,
                RoundName = item.ReviewRound.RoundName,

                AssignedDate = item.AssignedDate,
                TimeSlotId = item.TimeSlotId,
                SlotName = item.TimeSlot?.SlotName ?? string.Empty,
                StartTime = item.TimeSlot?.StartTime.ToString(@"hh\:mm") ?? string.Empty,
                EndTime = item.TimeSlot?.EndTime.ToString(@"hh\:mm") ?? string.Empty,

                ProjectGroupId = item.ReviewRound.ProjectGroupId,
                GroupCode = item.ReviewRound.ProjectGroup.GroupCode,
                GroupName = item.ReviewRound.ProjectGroup.GroupName,

                ProjectCourseId = item.ReviewRound.ProjectGroup.ProjectCourseId,
                CourseCode = item.ReviewRound.ProjectGroup.ProjectCourse.CourseCode,
                CourseName = item.ReviewRound.ProjectGroup.ProjectCourse.CourseName,

                SemesterId = item.ReviewRound.ProjectGroup.ProjectCourse.SemesterId,
                SemesterCode = item.ReviewRound.ProjectGroup.ProjectCourse.Semester.SemesterCode,
                SemesterName = item.ReviewRound.ProjectGroup.ProjectCourse.Semester.SemesterName,

                Location = item.Location,
                Status = item.Status,
                Note = item.Note,

                Teachers = item.ReviewAssignmentTeachers
                    .OrderBy(x => x.Teacher.TeacherCode)
                    .Select(x => new ScheduleOverviewTeacherDto
                    {
                        TeacherId = x.TeacherId,
                        TeacherCode = x.Teacher.TeacherCode,
                        FullName = x.Teacher.User.FullName,
                        Department = x.Teacher.Department,
                        RoleInPanel = x.RoleInPanel
                    })
                    .ToList()
            };
        }

        public async Task<(bool Success, string Message, List<ScheduleOverviewAssignmentDto>? Data)> GetMyScheduleAsync(
            long userId,
            DateOnly? fromDate = null,
            DateOnly? toDate = null)
        {
            var teacher = await _userRepository.GetTeacherByUserIdAsync(userId);
            if (teacher == null)
                return (false, "Không tìm thấy hồ sơ giảng viên.", null);

            var assignments = await _reviewAssignmentQueryRepository.GetMyScheduleAsync(teacher.TeacherId, fromDate, toDate);
            var data = assignments.Select(MapAssignmentDto).ToList();
            return (true, "OK", data);
        }
    }
}
