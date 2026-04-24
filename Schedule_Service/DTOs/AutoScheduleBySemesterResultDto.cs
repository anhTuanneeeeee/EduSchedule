using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.DTOs
{
    public class AutoScheduleBySemesterResultDto
    {
        public int TotalGroups { get; set; }
        public int ScheduledCount { get; set; }
        public int FailedCount { get; set; }

        public List<AutoScheduleBySemesterScheduledItemDto> ScheduledItems { get; set; } = new();
        public List<AutoScheduleBySemesterFailedItemDto> FailedItems { get; set; } = new();
    }

    public class AutoScheduleBySemesterScheduledItemDto
    {
        public long ReviewAssignmentId { get; set; }
        public long ReviewRoundId { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public DateOnly AssignedDate { get; set; }
        public long TimeSlotId { get; set; }
        public string SlotName { get; set; } = string.Empty;
        public List<string> TeacherCodes { get; set; } = new();
    }

    public class AutoScheduleBySemesterFailedItemDto
    {
        public long? ReviewRoundId { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
