using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class TimeSlotAutoScheduleRepository : ITimeSlotAutoScheduleRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public TimeSlotAutoScheduleRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSlot>> GetActiveAsync()
        {
            return await _context.TimeSlots
                .Where(x => x.IsActive)
                .AsNoTracking()
                .OrderBy(x => x.SlotNumber)
                .ToListAsync();
        }
    }
}
