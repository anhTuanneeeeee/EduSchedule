using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class ReviewRoundRepository : IReviewRoundRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public ReviewRoundRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewRound>> GetUnscheduledBySemesterAsync(long semesterId)
        {
            return await _context.ReviewRounds
                .Include(x => x.ProjectGroup)
                    .ThenInclude(x => x.ProjectCourse)
                        .ThenInclude(x => x.Semester)
                .Include(x => x.ProjectGroup)
                    .ThenInclude(x => x.ProjectSupervisors)
                .Include(x => x.ReviewAssignment)
                .Where(x =>
                    x.ProjectGroup.ProjectCourse.SemesterId == semesterId &&
                    x.ReviewAssignment == null)
                .AsNoTracking()
                .OrderBy(x => x.RoundNumber)
                .ThenBy(x => x.ProjectGroup.GroupCode)
                .ToListAsync();
        }

        public async Task<ReviewRound?> GetByIdWithDetailsAsync(long reviewRoundId)
        {
            return await _context.ReviewRounds
                .Include(x => x.ProjectGroup)
                    .ThenInclude(x => x.ProjectCourse)
                        .ThenInclude(x => x.Semester)
                .Include(x => x.ProjectGroup)
                    .ThenInclude(x => x.ProjectSupervisors)
                .Include(x => x.ReviewAssignment)
                .FirstOrDefaultAsync(x => x.ReviewRoundId == reviewRoundId);
        }
    }
}
