using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.DTOs
{
    public class ScheduleOverviewSlotDto
    {
        public long TimeSlotId { get; set; }
        public int SlotNumber { get; set; }
        public string SlotName { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public List<ScheduleOverviewAssignmentDto> Assignments { get; set; } = new();
    }
}
