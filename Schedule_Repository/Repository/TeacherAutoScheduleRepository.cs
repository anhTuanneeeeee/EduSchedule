using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class TeacherAutoScheduleRepository : ITeacherAutoScheduleRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public TeacherAutoScheduleRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<Teacher>> GetAvailableForReviewAsync()
        {
            return await _context.Teachers
                .Include(x => x.User)
                .Where(x => x.IsAvailableForProjectReview && x.User.IsActive)
                .AsNoTracking()
                .OrderBy(x => x.TeacherCode)
                .ToListAsync();
        }
    }
}
