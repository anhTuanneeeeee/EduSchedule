using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class ReviewAssignmentRepository : IReviewAssignmentRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public ReviewAssignmentRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByReviewRoundIdAsync(long reviewRoundId)
        {
            return await _context.ReviewAssignments.AnyAsync(x => x.ReviewRoundId == reviewRoundId);
        }

        public async Task<ReviewAssignment> CreateWithTeachersAsync(
            ReviewAssignment assignment,
            List<ReviewAssignmentTeacher> teachers)
        {
            assignment.ReviewAssignmentTeachers = teachers;
            _context.ReviewAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<ReviewAssignment?> GetByIdAsync(long reviewAssignmentId)
        {
            return await _context.ReviewAssignments
                .Include(x => x.TimeSlot)
                .Include(x => x.ReviewRound)
                    .ThenInclude(r => r.ProjectGroup)
                        .ThenInclude(g => g.ProjectCourse)
                            .ThenInclude(c => c.Semester)
                .Include(x => x.ReviewAssignmentTeachers)
                    .ThenInclude(t => t.Teacher)
                        .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(x => x.ReviewAssignmentId == reviewAssignmentId);
        }

        public async Task<bool> UpdateAsync(ReviewAssignment assignment)
        {
            _context.ReviewAssignments.Update(assignment);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
