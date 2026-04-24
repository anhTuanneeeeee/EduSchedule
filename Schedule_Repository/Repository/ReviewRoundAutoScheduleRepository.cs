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
    public class ReviewRoundAutoScheduleRepository : IReviewRoundAutoScheduleRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public ReviewRoundAutoScheduleRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<ReviewRound?> GetByProjectGroupAndRoundNumberAsync(long projectGroupId, int roundNumber)
        {
            return await _context.ReviewRounds
                .Include(x => x.ProjectGroup)
                    .ThenInclude(x => x.ProjectCourse)
                        .ThenInclude(x => x.Semester)
                .Include(x => x.ProjectGroup)
                    .ThenInclude(x => x.ProjectSupervisors)
                .Include(x => x.ReviewAssignment)
                .FirstOrDefaultAsync(x =>
                    x.ProjectGroupId == projectGroupId &&
                    x.RoundNumber == roundNumber);
        }

        public async Task<ReviewRound> CreateAsync(ReviewRound reviewRound)
        {
            _context.ReviewRounds.Add(reviewRound);
            await _context.SaveChangesAsync();
            return reviewRound;
        }
    }
}