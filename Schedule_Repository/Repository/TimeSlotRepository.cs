using Microsoft.EntityFrameworkCore;
using Schedule_Repository.IRepository;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public TimeSlotRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<TimeSlot?> GetByIdAsync(long timeSlotId)
        {
            return await _context.TimeSlots
                .FirstOrDefaultAsync(x => x.TimeSlotId == timeSlotId);
        }
    }
}
