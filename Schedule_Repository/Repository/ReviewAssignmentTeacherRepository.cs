using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class ReviewAssignmentTeacherRepository : IReviewAssignmentTeacherRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public ReviewAssignmentTeacherRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewAssignmentTeacher>> GetBySemesterAsync(long semesterId)
        {
            return await _context.ReviewAssignmentTeachers
                .Include(x => x.Teacher)
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

        public async Task<bool> HasTeacherConflictSameSlotAsync(long teacherId, DateOnly assignedDate, long timeSlotId)
        {
            return await _context.ReviewAssignmentTeachers.AnyAsync(x =>
                x.TeacherId == teacherId &&
                x.ReviewAssignment.AssignedDate == assignedDate &&
                x.ReviewAssignment.TimeSlotId == timeSlotId);
        }

        public async Task<int> CountTeacherAssignmentsOnDateAsync(long teacherId, DateOnly assignedDate)
        {
            return await _context.ReviewAssignmentTeachers.CountAsync(x =>
                x.TeacherId == teacherId &&
                x.ReviewAssignment.AssignedDate == assignedDate);
        }
    }
}
