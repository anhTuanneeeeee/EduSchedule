using Schedule_Repository.IRepository;
using Schedule_Repository.Models;
using Schedule_Service.DTOs;
using Schedule_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.Service
{
    public class ProjectGroupService : IProjectGroupService
    {
        private readonly IProjectGroupRepository _projectGroupRepository;
        private readonly IProjectCourseRepository _projectCourseRepository;

        public ProjectGroupService(IProjectGroupRepository projectGroupRepository, IProjectCourseRepository projectCourseRepository)
        {
            _projectGroupRepository = projectGroupRepository;
            _projectCourseRepository = projectCourseRepository;
        }

        public async Task<List<ProjectGroupResponseDto>> GetAllAsync()
        {
            var groups = await _projectGroupRepository.GetAllAsync();
            return groups.Select(MapToResponse).ToList();
        }

        public async Task<ProjectGroupResponseDto?> GetByIdAsync(long projectGroupId)
        {
            var group = await _projectGroupRepository.GetByIdAsync(projectGroupId);
            return group == null ? null : MapToResponse(group);
        }

        public async Task<List<ProjectGroupResponseDto>> GetByProjectCourseIdAsync(long projectCourseId)
        {
            var groups = await _projectGroupRepository.GetByProjectCourseIdAsync(projectCourseId);
            return groups.Select(MapToResponse).ToList();
        }

        public async Task<(bool Success, string Message, ProjectGroupResponseDto? Data)> CreateAsync(CreateProjectGroupRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.GroupCode))
                return (false, "GroupCode không được để trống.", null);

            if (string.IsNullOrWhiteSpace(request.GroupName))
                return (false, "GroupName không được để trống.", null);

            var course = await _projectCourseRepository.GetByIdAsync(request.ProjectCourseId);
            if (course == null)
                return (false, "Không tìm thấy ProjectCourse.", null);

            bool codeExists = await _projectGroupRepository.GroupCodeExistsAsync(request.GroupCode.Trim(), request.ProjectCourseId);
            if (codeExists)
                return (false, "GroupCode đã tồn tại trong ProjectCourse này.", null);

            var projectGroup = new ProjectGroup
            {
                ProjectCourseId = request.ProjectCourseId,
                GroupCode = request.GroupCode.Trim(),
                GroupName = request.GroupName.Trim(),
                Status = request.Status.Trim()
            };

            var created = await _projectGroupRepository.CreateAsync(projectGroup);

            return (true, "Tạo ProjectGroup thành công.", MapToResponse(created));
        }

        public async Task<(bool Success, string Message)> UpdateAsync(long projectGroupId, UpdateProjectGroupRequestDto request)
        {
            var group = await _projectGroupRepository.GetByIdAsync(projectGroupId);
            if (group == null)
                return (false, "Không tìm thấy ProjectGroup.");

            if (string.IsNullOrWhiteSpace(request.GroupCode))
                return (false, "GroupCode không được để trống.");

            if (string.IsNullOrWhiteSpace(request.GroupName))
                return (false, "GroupName không được để trống.");

            bool codeExists = await _projectGroupRepository.GroupCodeExistsAsync(request.GroupCode.Trim(), group.ProjectCourseId, projectGroupId);
            if (codeExists)
                return (false, "GroupCode đã tồn tại trong ProjectCourse này.");

            group.GroupCode = request.GroupCode.Trim();
            group.GroupName = request.GroupName.Trim();
            group.Status = request.Status.Trim();

            bool result = await _projectGroupRepository.UpdateAsync(group);

            return result
                ? (true, "Cập nhật ProjectGroup thành công.")
                : (false, "Cập nhật ProjectGroup thất bại.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(long projectGroupId)
        {
            var group = await _projectGroupRepository.GetByIdAsync(projectGroupId);
            if (group == null)
                return (false, "Không tìm thấy ProjectGroup.");

            bool hasSupervisors = await _projectGroupRepository.HasSupervisorsAsync(projectGroupId);
            if (hasSupervisors)
                return (false, "ProjectGroup đã có ProjectSupervisor, không thể xóa.");

            bool hasReviewRounds = await _projectGroupRepository.HasReviewRoundsAsync(projectGroupId);
            if (hasReviewRounds)
                return (false, "ProjectGroup đã có ReviewRound, không thể xóa.");
            
            bool result = await _projectGroupRepository.DeleteAsync(group);

            return result
                ? (true, "Xóa ProjectGroup thành công.")
                : (false, "Xóa ProjectGroup thất bại.");
        }

        private static ProjectGroupResponseDto MapToResponse(ProjectGroup group)
        {
            return new ProjectGroupResponseDto
            {
                ProjectGroupId = group.ProjectGroupId,
                ProjectCourseId = group.ProjectCourseId,
                GroupCode = group.GroupCode,
                GroupName = group.GroupName,
                Status = group.Status
            };
        }
    }
}
