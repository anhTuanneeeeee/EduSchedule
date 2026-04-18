using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.IService
{
    public interface IProjectGroupService
    {
        Task<List<ProjectGroupResponseDto>> GetAllAsync();
        Task<ProjectGroupResponseDto?> GetByIdAsync(long projectGroupId);
        Task<List<ProjectGroupResponseDto>> GetByProjectCourseIdAsync(long projectCourseId);
        Task<(bool Success, string Message, ProjectGroupResponseDto? Data)> CreateAsync(CreateProjectGroupRequestDto request);
        Task<(bool Success, string Message)> UpdateAsync(long projectGroupId, UpdateProjectGroupRequestDto request);
        Task<(bool Success, string Message)> DeleteAsync(long projectGroupId);
    }
}
