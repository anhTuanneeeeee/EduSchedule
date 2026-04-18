using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.IRepository
{
    public interface IProjectGroupRepository
    {
        Task<List<ProjectGroup>> GetAllAsync();
        Task<ProjectGroup?> GetByIdAsync(long projectGroupId);
        Task<List<ProjectGroup>> GetByProjectCourseIdAsync(long projectCourseId);
        Task<bool> GroupCodeExistsAsync(string groupCode, long projectCourseId, long? excludeProjectGroupId = null);
        Task<bool> HasSupervisorsAsync(long projectGroupId);
        Task<bool> HasReviewRoundsAsync(long projectGroupId);
        Task<ProjectGroup> CreateAsync(ProjectGroup projectGroup);
        Task<bool> UpdateAsync(ProjectGroup projectGroup);
        Task<bool> DeleteAsync(ProjectGroup projectGroup);
    }
}
