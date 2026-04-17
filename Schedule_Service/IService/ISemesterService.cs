using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.IService
{
    public interface ISemesterService
    {
        Task<List<SemesterResponseDto>> GetAllAsync();
        Task<SemesterResponseDto?> GetByIdAsync(long semesterId);
        Task<(bool Success, string Message, SemesterResponseDto? Data)> CreateAsync(CreateSemesterRequestDto request);
        Task<(bool Success, string Message)> UpdateAsync(long semesterId, UpdateSemesterRequestDto request);
        Task<(bool Success, string Message)> DeleteAsync(long semesterId);
    }
}
