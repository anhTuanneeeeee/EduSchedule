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
    public class ProjectCourseService : IProjectCourseService
    {
        private readonly IProjectCourseRepository _projectCourseRepository;
        private readonly ISemesterRepository _semesterRepository;

        public ProjectCourseService(IProjectCourseRepository projectCourseRepository, ISemesterRepository semesterRepository)
        {
            _projectCourseRepository = projectCourseRepository;
            _semesterRepository = semesterRepository;
        }

        public async Task<List<ProjectCourseResponseDto>> GetAllAsync()
        {
            var courses = await _projectCourseRepository.GetAllAsync();
            return courses.Select(MapToResponse).ToList();
        }

        public async Task<ProjectCourseResponseDto?> GetByIdAsync(long projectCourseId)
        {
            var course = await _projectCourseRepository.GetByIdAsync(projectCourseId);
            return course == null ? null : MapToResponse(course);
        }

        public async Task<List<ProjectCourseResponseDto>> GetBySemesterIdAsync(long semesterId)
        {
            var courses = await _projectCourseRepository.GetBySemesterIdAsync(semesterId);
            return courses.Select(MapToResponse).ToList();
        }

        public async Task<(bool Success, string Message, ProjectCourseResponseDto? Data)> CreateAsync(CreateProjectCourseRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.CourseCode))
                return (false, "CourseCode không được để trống.", null);

            if (string.IsNullOrWhiteSpace(request.CourseName))
                return (false, "CourseName không được để trống.", null);

            var semester = await _semesterRepository.GetByIdAsync(request.SemesterId);
            if (semester == null)
                return (false, "Không tìm thấy semester.", null);

            bool codeExists = await _projectCourseRepository.CourseCodeExistsAsync(request.CourseCode.Trim());
            if (codeExists)
                return (false, "CourseCode đã tồn tại.", null);

            var projectCourse = new ProjectCourse
            {
                SemesterId = request.SemesterId,
                CourseCode = request.CourseCode.Trim(),
                CourseName = request.CourseName.Trim(),
                Description = request.Description?.Trim()
            };

            var created = await _projectCourseRepository.CreateAsync(projectCourse);

            return (true, "Tạo ProjectCourse thành công.", MapToResponse(created));
        }

        public async Task<(bool Success, string Message)> UpdateAsync(long projectCourseId, UpdateProjectCourseRequestDto request)
        {
            var course = await _projectCourseRepository.GetByIdAsync(projectCourseId);
            if (course == null)
                return (false, "Không tìm thấy ProjectCourse.");

            if (string.IsNullOrWhiteSpace(request.CourseCode))
                return (false, "CourseCode không được để trống.");

            if (string.IsNullOrWhiteSpace(request.CourseName))
                return (false, "CourseName không được để trống.");

            bool codeExists = await _projectCourseRepository.CourseCodeExistsAsync(request.CourseCode.Trim(), projectCourseId);
            if (codeExists)
                return (false, "CourseCode đã tồn tại.");

            course.CourseCode = request.CourseCode.Trim();
            course.CourseName = request.CourseName.Trim();
            course.Description = request.Description?.Trim();

            bool result = await _projectCourseRepository.UpdateAsync(course);

            return result
                ? (true, "Cập nhật ProjectCourse thành công.")
                : (false, "Cập nhật ProjectCourse thất bại.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(long projectCourseId)
        {
            var course = await _projectCourseRepository.GetByIdAsync(projectCourseId);
            if (course == null)
                return (false, "Không tìm thấy ProjectCourse.");

            bool hasGroups = await _projectCourseRepository.HasProjectGroupsAsync(projectCourseId);
            if (hasGroups)
                return (false, "ProjectCourse đã có ProjectGroup, không thể xóa.");

            bool result = await _projectCourseRepository.DeleteAsync(course);

            return result
                ? (true, "Xóa ProjectCourse thành công.")
                : (false, "Xóa ProjectCourse thất bại.");
        }

        private static ProjectCourseResponseDto MapToResponse(ProjectCourse course)
        {
            return new ProjectCourseResponseDto
            {
                ProjectCourseId = course.ProjectCourseId,
                SemesterId = course.SemesterId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description
            };
        }
    }
}
