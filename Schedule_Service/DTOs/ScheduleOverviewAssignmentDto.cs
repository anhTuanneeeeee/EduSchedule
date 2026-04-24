using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.DTOs
{
    public class ScheduleOverviewAssignmentDto
    {
        public long ReviewAssignmentId { get; set; }
        public long ReviewRoundId { get; set; }
        public int RoundNumber { get; set; }
        public string? RoundName { get; set; }

        // Date & Slot (flat fields for my-schedule endpoint)
        public DateOnly AssignedDate { get; set; }
        public long TimeSlotId { get; set; }
        public string SlotName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;

        public long ProjectGroupId { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;

        public long ProjectCourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;

        public long SemesterId { get; set; }
        public string SemesterCode { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;

        public string? Location { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }

        public List<ScheduleOverviewTeacherDto> Teachers { get; set; } = new();
    }
}
