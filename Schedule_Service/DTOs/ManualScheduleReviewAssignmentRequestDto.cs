using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.DTOs
{
    public class ManualScheduleReviewAssignmentRequestDto
    {
        public long ProjectGroupId { get; set; }
        public int RoundNumber { get; set; } = 1;
        public string? RoundName { get; set; }

        public DateOnly AssignedDate { get; set; }
        public long TimeSlotId { get; set; }
        public List<long> TeacherIds { get; set; } = new();

        public string Status { get; set; } = "OPEN";
        public string? Location { get; set; }
        public string? Note { get; set; }
    }
}
