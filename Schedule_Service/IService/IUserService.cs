using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.IService
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetByIdAsync(int userId);
        Task<List<RoleResponseDto>?> GetRolesByUserIdAsync(int userId);
        Task<List<UserResponseDto>> GetUsersByRoleNameAsync(string roleName);

        Task<(bool Success, string Message)> UpdateAsync(int userId, UpdateUserRequestDto request);
        Task<(bool Success, string Message)> DeleteAsync(int userId);
        Task<(bool Success, string Message)> AssignRoleAsync(int userId, int roleId);
        Task<(bool Success, string Message)> RemoveRoleAsync(int userId, int roleId);
    }
}
