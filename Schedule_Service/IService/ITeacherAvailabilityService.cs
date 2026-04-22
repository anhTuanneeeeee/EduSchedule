using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.IService
{
    public interface ITeacherAvailabilityService
    {
        Task<(bool Success, string Message, List<TeacherAvailabilityResponseDto>? Data)> GetMyAsync(
            long userId,
            DateOnly? fromDate = null,
            DateOnly? toDate = null);
        Task<(bool Success, string Message, List<TeacherAvailabilityResponseDto>? Data)> GetByTeacherCodeAsync(
            string teacherCode,
            DateOnly? fromDate = null,
            DateOnly? toDate = null);

        Task<(bool Success, string Message, TeacherAvailabilityResponseDto? Data)> CreateAsync(
            long userId,
            CreateTeacherAvailabilityRequestDto request);

        Task<(bool Success, string Message)> UpdateAsync(
            long userId,
            long teacherAvailabilityId,
            UpdateTeacherAvailabilityRequestDto request);

        Task<(bool Success, string Message)> DeleteAsync(
            long userId,
            long teacherAvailabilityId);
    }
}
