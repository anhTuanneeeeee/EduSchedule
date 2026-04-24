using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class ReviewAssignmentTeacherAutoScheduleRepository : IReviewAssignmentTeacherAutoScheduleRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public ReviewAssignmentTeacherAutoScheduleRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewAssignmentTeacher>> GetBySemesterAsync(long semesterId)
        {
            return await _context.ReviewAssignmentTeachers
                .Include(x => x.ReviewAssignment)
                    .ThenInclude(x => x.TimeSlot)
                .Include(x => x.ReviewAssignment)
                    .ThenInclude(x => x.ReviewRound)
                        .ThenInclude(x => x.ProjectGroup)
                            .ThenInclude(x => x.ProjectCourse)
                .Where(x => x.ReviewAssignment.ReviewRound.ProjectGroup.ProjectCourse.SemesterId == semesterId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
