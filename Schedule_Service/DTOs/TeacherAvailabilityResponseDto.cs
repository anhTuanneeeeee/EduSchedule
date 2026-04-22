using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.DTOs
{
    public class TeacherAvailabilityResponseDto
    {
        public long TeacherAvailabilityId { get; set; }
        public long TeacherId { get; set; }
        public DateOnly AvailableDate { get; set; }
        public long TimeSlotId { get; set; }
        public int SlotNumber { get; set; }
        public string SlotName { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string AvailabilityStatus { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
