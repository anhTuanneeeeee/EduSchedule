using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class TeacherAvailabilityAutoScheduleRepository : ITeacherAvailabilityAutoScheduleRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public TeacherAvailabilityAutoScheduleRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<TeacherAvailability>> GetAvailableInRangeAsync(
            DateOnly fromDate,
            DateOnly toDate,
            List<long>? timeSlotIds = null)
        {
            var query = _context.TeacherAvailabilities
                .Where(x =>
                    x.AvailableDate >= fromDate &&
                    x.AvailableDate <= toDate &&
                    (x.AvailabilityStatus == "AVAILABLE" || x.AvailabilityStatus == "PREFERRED"))
                .AsNoTracking();

            if (timeSlotIds != null && timeSlotIds.Any())
            {
                query = query.Where(x => timeSlotIds.Contains(x.TimeSlotId));
            }

            return await query.ToListAsync();
        }
    }
}
