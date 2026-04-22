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
    public class TeacherAvailabilityRepository : ITeacherAvailabilityRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public TeacherAvailabilityRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<TeacherAvailability>> GetByTeacherIdAsync(
            long teacherId,
            DateOnly? fromDate = null,
            DateOnly? toDate = null)
        {
            var query = _context.TeacherAvailabilities
                .Include(x => x.TimeSlot)
                .Where(x => x.TeacherId == teacherId)
                .AsNoTracking();

            if (fromDate.HasValue)
                query = query.Where(x => x.AvailableDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.AvailableDate <= toDate.Value);

            return await query
                .OrderBy(x => x.AvailableDate)
                .ThenBy(x => x.TimeSlot.SlotNumber)
                .ToListAsync();
        }

        public async Task<TeacherAvailability?> GetByIdAsync(long teacherAvailabilityId)
        {
            return await _context.TeacherAvailabilities
                .Include(x => x.TimeSlot)
                .FirstOrDefaultAsync(x => x.TeacherAvailabilityId == teacherAvailabilityId);
        }

        public async Task<bool> ExistsDuplicateAsync(
            long teacherId,
            DateOnly availableDate,
            long timeSlotId,
            long? excludeId = null)
        {
            return await _context.TeacherAvailabilities.AnyAsync(x =>
                x.TeacherId == teacherId &&
                x.AvailableDate == availableDate &&
                x.TimeSlotId == timeSlotId &&
                (!excludeId.HasValue || x.TeacherAvailabilityId != excludeId.Value));
        }

        public async Task<TeacherAvailability> CreateAsync(TeacherAvailability entity)
        {
            _context.TeacherAvailabilities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(TeacherAvailability entity)
        {
            _context.TeacherAvailabilities.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(TeacherAvailability entity)
        {
            _context.TeacherAvailabilities.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
