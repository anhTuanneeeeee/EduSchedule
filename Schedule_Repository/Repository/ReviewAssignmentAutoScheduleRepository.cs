using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class ReviewAssignmentAutoScheduleRepository : IReviewAssignmentAutoScheduleRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public ReviewAssignmentAutoScheduleRepository(ScheduleForTeacherContext context)
        {
            _context = context;
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
    }
}