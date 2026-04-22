using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.DTOs
{
    public class UpdateTeacherAvailabilityRequestDto
    {
        public DateOnly AvailableDate { get; set; }
        public long TimeSlotId { get; set; }
        public string AvailabilityStatus { get; set; } = "AVAILABLE";
        public string? Note { get; set; }
    }
}
