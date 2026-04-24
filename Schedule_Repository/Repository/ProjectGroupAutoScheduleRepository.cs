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
    public class ProjectGroupAutoScheduleRepository : IProjectGroupAutoScheduleRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public ProjectGroupAutoScheduleRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectGroup>> GetBySemesterAsync(long semesterId)
        {
            return await _context.ProjectGroups
                .Include(x => x.ProjectCourse)
                    .ThenInclude(x => x.Semester)
                .Include(x => x.ProjectSupervisors)
                .Include(x => x.ReviewRounds)
                    .ThenInclude(x => x.ReviewAssignment)
                .Where(x => x.ProjectCourse.SemesterId == semesterId)
                .AsNoTracking()
                .OrderBy(x => x.GroupCode)
                .ToListAsync();
        }
    }
}
