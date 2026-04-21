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
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;

        public SemesterService(ISemesterRepository semesterRepository)
        {
            _semesterRepository = semesterRepository;
        }

        public async Task<List<SemesterResponseDto>> GetAllAsync()
        {
            var semesters = await _semesterRepository.GetAllAsync();
            return semesters.Select(MapToResponse).ToList();
        }

        public async Task<SemesterResponseDto?> GetByIdAsync(long semesterId)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            return semester == null ? null : MapToResponse(semester);
        }

        public async Task<(bool Success, string Message, SemesterResponseDto? Data)> CreateAsync(CreateSemesterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.SemesterCode))
                return (false, "SemesterCode không được để trống.", null);

            if (string.IsNullOrWhiteSpace(request.SemesterName))
                return (false, "SemesterName không được để trống.", null);

            if (string.IsNullOrWhiteSpace(request.AcademicYear))
                return (false, "AcademicYear không được để trống.", null);

            if (request.TermNumber <= 0)
                return (false, "TermNumber phải lớn hơn 0.", null);

            if (request.StartDate >= request.EndDate)
                return (false, "StartDate phải nhỏ hơn EndDate.", null);

            bool codeExists = await _semesterRepository.SemesterCodeExistsAsync(request.SemesterCode.Trim());
            if (codeExists)
                return (false, "SemesterCode đã tồn tại.", null);

            bool nameExists = await _semesterRepository.SemesterNameExistsAsync(request.SemesterName.Trim());
            if (nameExists)
                return (false, "SemesterName đã tồn tại.", null);

            var semester = new Semester
            {
                SemesterCode = request.SemesterCode.Trim(),
                SemesterName = request.SemesterName.Trim(),
                AcademicYear = request.AcademicYear.Trim(),
                TernNumber = request.TermNumber,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive
            };

            var created = await _semesterRepository.CreateAsync(semester);

            return (true, "Tạo semester thành công.", MapToResponse(created));
        }

        public async Task<(bool Success, string Message)> UpdateAsync(long semesterId, UpdateSemesterRequestDto request)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            if (semester == null)
                return (false, "Không tìm thấy semester.");

            if (string.IsNullOrWhiteSpace(request.SemesterCode))
                return (false, "SemesterCode không được để trống.");

            if (string.IsNullOrWhiteSpace(request.SemesterName))
                return (false, "SemesterName không được để trống.");

            if (string.IsNullOrWhiteSpace(request.AcademicYear))
                return (false, "AcademicYear không được để trống.");

            if (request.TermNumber <= 0)
                return (false, "TermNumber phải lớn hơn 0.");

            if (request.StartDate >= request.EndDate)
                return (false, "StartDate phải nhỏ hơn EndDate.");

            bool codeExists = await _semesterRepository.SemesterCodeExistsAsync(request.SemesterCode.Trim(), semesterId);
            if (codeExists)
                return (false, "SemesterCode đã tồn tại.");

            bool nameExists = await _semesterRepository.SemesterNameExistsAsync(request.SemesterName.Trim(), semesterId);
            if (nameExists)
                return (false, "SemesterName đã tồn tại.");

            semester.SemesterCode = request.SemesterCode.Trim();
            semester.SemesterName = request.SemesterName.Trim();
            semester.AcademicYear = request.AcademicYear.Trim();
            semester.TernNumber = request.TermNumber;
            semester.StartDate = request.StartDate;
            semester.EndDate = request.EndDate;
            semester.IsActive = request.IsActive;

            bool result = await _semesterRepository.UpdateAsync(semester);

            return result
                ? (true, "Cập nhật semester thành công.")
                : (false, "Cập nhật semester thất bại.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(long semesterId)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            if (semester == null)
                return (false, "Không tìm thấy semester.");

            bool hasProjectCourses = await _semesterRepository.HasProjectCoursesAsync(semesterId);
            if (hasProjectCourses)
                return (false, "Semester đã có ProjectCourse, không thể xóa.");

            bool result = await _semesterRepository.DeleteAsync(semester);

            return result
                ? (true, "Xóa semester thành công.")
                : (false, "Xóa semester thất bại.");
        }

        private static SemesterResponseDto MapToResponse(Semester semester)
        {
            return new SemesterResponseDto
            {
                SemesterId = semester.SemesterId,
                SemesterCode = semester.SemesterCode,
                SemesterName = semester.SemesterName,
                AcademicYear = semester.AcademicYear,
                TermNumber = (int)semester.TernNumber,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate,
                IsActive = semester.IsActive
            };
        }
    }
}
