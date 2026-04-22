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
        public async Task<List<TimeSlot>> GetActiveAsync(List<long>? timeSlotIds = null)
        {
            var query = _context.TimeSlots
                .Where(x => x.IsActive)
                .AsNoTracking();

            if (timeSlotIds != null && timeSlotIds.Any())
            {
                query = query.Where(x => timeSlotIds.Contains(x.TimeSlotId));
            }

            return await query
                .OrderBy(x => x.SlotNumber)
                .ToListAsync();
        }
    }
}
