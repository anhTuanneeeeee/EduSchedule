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

        public async Task<List<ReviewAssignment>> GetScheduleOverviewAsync(
            long semesterId,
            DateOnly? fromDate = null,
            DateOnly? toDate = null,
            string? status = null)
        {
            var query = _context.ReviewAssignments
                .Include(x => x.TimeSlot)
                .Include(x => x.ReviewAssignmentTeachers)
                    .ThenInclude(x => x.Teacher)
                        .ThenInclude(x => x.User)
                .Include(x => x.ReviewRound)
                    .ThenInclude(x => x.ProjectGroup)
                        .ThenInclude(x => x.ProjectCourse)
                            .ThenInclude(x => x.Semester)
                .Where(x => x.ReviewRound.ProjectGroup.ProjectCourse.SemesterId == semesterId)
                .AsNoTracking();

            if (fromDate.HasValue)
                query = query.Where(x => x.AssignedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.AssignedDate <= toDate.Value);

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = status.Trim().ToUpperInvariant();
                query = query.Where(x => x.Status.ToUpper() == normalizedStatus);
            }

            return await query
                .OrderBy(x => x.AssignedDate)
                .ThenBy(x => x.TimeSlot.SlotNumber)
                .ThenBy(x => x.ReviewRound.ProjectGroup.GroupCode)
                .ToListAsync();
        }

        public async Task<List<ReviewAssignment>> GetMyScheduleAsync(long teacherId, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            var query = _context.ReviewAssignments
                .Include(x => x.TimeSlot)
                .Include(x => x.ReviewAssignmentTeachers)
                    .ThenInclude(x => x.Teacher)
                        .ThenInclude(x => x.User)
                .Include(x => x.ReviewRound)
                    .ThenInclude(x => x.ProjectGroup)
                        .ThenInclude(x => x.ProjectCourse)
                            .ThenInclude(x => x.Semester)
                .Where(x => x.ReviewAssignmentTeachers.Any(t => t.TeacherId == teacherId))
                .AsNoTracking();

            if (fromDate.HasValue)
                query = query.Where(x => x.AssignedDate >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(x => x.AssignedDate <= toDate.Value);

            return await query
                .OrderBy(x => x.AssignedDate)
                .ThenBy(x => x.TimeSlot.SlotNumber)
                .ToListAsync();
        }
    }
}
