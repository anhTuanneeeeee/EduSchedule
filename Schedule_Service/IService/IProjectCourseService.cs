using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.IService
{
    public interface IProjectCourseService
    {
        Task<List<ProjectCourseResponseDto>> GetAllAsync();
        Task<ProjectCourseResponseDto?> GetByIdAsync(long projectCourseId);
        Task<List<ProjectCourseResponseDto>> GetBySemesterIdAsync(long semesterId);
        Task<(bool Success, string Message, ProjectCourseResponseDto? Data)> CreateAsync(CreateProjectCourseRequestDto request);
        Task<(bool Success, string Message)> UpdateAsync(long projectCourseId, UpdateProjectCourseRequestDto request);
        Task<(bool Success, string Message)> DeleteAsync(long projectCourseId);
    }
}
