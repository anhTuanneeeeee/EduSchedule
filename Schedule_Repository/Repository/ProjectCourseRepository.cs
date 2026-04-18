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
    public class ProjectCourseRepository : IProjectCourseRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public ProjectCourseRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectCourse>> GetAllAsync()
        {
            return await _context.ProjectCourses
                .AsNoTracking()
                .OrderByDescending(x => x.ProjectCourseId)
                .ToListAsync();
        }

        public async Task<ProjectCourse?> GetByIdAsync(long projectCourseId)
        {
            return await _context.ProjectCourses
                .FirstOrDefaultAsync(x => x.ProjectCourseId == projectCourseId);
        }

        public async Task<List<ProjectCourse>> GetBySemesterIdAsync(long semesterId)
        {
            return await _context.ProjectCourses
                .Where(x => x.SemesterId == semesterId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> CourseCodeExistsAsync(string courseCode, long? excludeProjectCourseId = null)
        {
            return await _context.ProjectCourses.AnyAsync(x =>
                x.CourseCode == courseCode &&
                (!excludeProjectCourseId.HasValue || x.ProjectCourseId != excludeProjectCourseId.Value));
        }

        public async Task<bool> HasProjectGroupsAsync(long projectCourseId)
        {
            return await _context.ProjectGroups.AnyAsync(x => x.ProjectCourseId == projectCourseId);
        }

        public async Task<ProjectCourse> CreateAsync(ProjectCourse projectCourse)
        {
            _context.ProjectCourses.Add(projectCourse);
            await _context.SaveChangesAsync();
            return projectCourse;
        }

        public async Task<bool> UpdateAsync(ProjectCourse projectCourse)
        {
            _context.ProjectCourses.Update(projectCourse);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(ProjectCourse projectCourse)
        {
            _context.ProjectCourses.Remove(projectCourse);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
