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
    public class ProjectGroupRepository : IProjectGroupRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public ProjectGroupRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectGroup>> GetAllAsync()
        {
            return await _context.ProjectGroups
                .AsNoTracking()
                .OrderByDescending(x => x.ProjectGroupId)
                .ToListAsync();
        }

        public async Task<ProjectGroup?> GetByIdAsync(long projectGroupId)
        {
            return await _context.ProjectGroups
                .FirstOrDefaultAsync(x => x.ProjectGroupId == projectGroupId);
        }

        public async Task<List<ProjectGroup>> GetByProjectCourseIdAsync(long projectCourseId)
        {
            return await _context.ProjectGroups
                .Where(x => x.ProjectCourseId == projectCourseId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> GroupCodeExistsAsync(string groupCode, long projectCourseId, long? excludeProjectGroupId = null)
        {
            return await _context.ProjectGroups.AnyAsync(x =>
                x.GroupCode == groupCode &&
                x.ProjectCourseId == projectCourseId &&
                (!excludeProjectGroupId.HasValue || x.ProjectGroupId != excludeProjectGroupId.Value));
        }

        public async Task<bool> HasSupervisorsAsync(long projectGroupId)
        {
            return await _context.ProjectSupervisors.AnyAsync(x => x.ProjectGroupId == projectGroupId);
        }

        public async Task<bool> HasReviewRoundsAsync(long projectGroupId)
        {
            return await _context.ReviewRounds.AnyAsync(x => x.ProjectGroupId == projectGroupId);
        }

        public async Task<ProjectGroup> CreateAsync(ProjectGroup projectGroup)
        {
            _context.ProjectGroups.Add(projectGroup);
            await _context.SaveChangesAsync();
            return projectGroup;
        }

        public async Task<bool> UpdateAsync(ProjectGroup projectGroup)
        {
            _context.ProjectGroups.Update(projectGroup);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(ProjectGroup projectGroup)
        {
            _context.ProjectGroups.Remove(projectGroup);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<ProjectGroup?> GetByIdWithDetailsAsync(long projectGroupId)
        {
            return await _context.ProjectGroups
                .Include(x => x.ProjectCourse)
                    .ThenInclude(x => x.Semester)
                .Include(x => x.ProjectSupervisors)
                .Include(x => x.ReviewRounds)
                    .ThenInclude(x => x.ReviewAssignment)
                .FirstOrDefaultAsync(x => x.ProjectGroupId == projectGroupId);
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
