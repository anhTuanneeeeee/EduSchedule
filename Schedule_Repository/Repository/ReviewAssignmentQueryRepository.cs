using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class ReviewAssignmentQueryRepository : IReviewAssignmentQueryRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public ReviewAssignmentQueryRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewAssignment>> GetScheduleOverviewAsync(long semesterId)
        {
            return await _context.ReviewAssignments
                .Include(x => x.TimeSlot)
                .Include(x => x.ReviewAssignmentTeachers)
                    .ThenInclude(x => x.Teacher)
                        .ThenInclude(x => x.User)
                .Include(x => x.ReviewRound)
                    .ThenInclude(x => x.ProjectGroup)
                        .ThenInclude(x => x.ProjectCourse)
                            .ThenInclude(x => x.Semester)
                .Where(x => x.ReviewRound.ProjectGroup.ProjectCourse.SemesterId == semesterId)
                .AsNoTracking()
                .OrderBy(x => x.AssignedDate)
                .ThenBy(x => x.TimeSlot.SlotNumber)
                .ThenBy(x => x.ReviewRound.ProjectGroup.GroupCode)
                .ToListAsync();
        }
    }
}
