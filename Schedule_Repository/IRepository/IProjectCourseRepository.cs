using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.IRepository
{
    public interface IProjectCourseRepository
    {
        Task<List<ProjectCourse>> GetAllAsync();
        Task<ProjectCourse?> GetByIdAsync(long projectCourseId);
        Task<List<ProjectCourse>> GetBySemesterIdAsync(long semesterId);
        Task<bool> CourseCodeExistsAsync(string courseCode, long? excludeProjectCourseId = null);
        Task<bool> HasProjectGroupsAsync(long projectCourseId);
        Task<ProjectCourse> CreateAsync(ProjectCourse projectCourse);
        Task<bool> UpdateAsync(ProjectCourse projectCourse);
        Task<bool> DeleteAsync(ProjectCourse projectCourse);
    }
}
