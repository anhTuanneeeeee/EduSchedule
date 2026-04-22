using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.DTOs
{
    public class AutoScheduleReviewAssignmentsRequestDto
    {
        public long SemesterId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public List<long>? TimeSlotIds { get; set; }
        public int TeachersPerAssignment { get; set; } = 2;
        public string Status { get; set; } = "OPEN";
        public string? DefaultLocation { get; set; }
        public string? Note { get; set; }
    }
}
