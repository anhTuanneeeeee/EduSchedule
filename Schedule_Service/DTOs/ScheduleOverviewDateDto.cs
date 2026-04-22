using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.DTOs
{
    public class ScheduleOverviewDateDto
    {
        public DateOnly AssignedDate { get; set; }
        public List<ScheduleOverviewSlotDto> Slots { get; set; } = new();
    }
}
