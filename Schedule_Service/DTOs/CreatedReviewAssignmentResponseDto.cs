using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.DTOs
{
    public class CreatedReviewAssignmentResponseDto
    {
        public long ReviewAssignmentId { get; set; }
        public long ReviewRoundId { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public DateOnly AssignedDate { get; set; }
        public long TimeSlotId { get; set; }
        public string SlotName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Location { get; set; }
        public List<string> TeacherCodes { get; set; } = new();
    }
}
