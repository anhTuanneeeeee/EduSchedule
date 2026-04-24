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
        Task<UserResponseDto?> GetByIdAsync(long userId);
        Task<List<RoleResponseDto>?> GetRolesByUserIdAsync(long userId);
        Task<List<UserResponseDto>> GetUsersByRoleNameAsync(string roleName);

        Task<(bool Success, string Message)> UpdateAsync(long userId, UpdateUserRequestDto request);
        Task<(bool Success, string Message)> DeleteAsync(long userId);
        Task<(bool Success, string Message)> AssignRoleAsync(long userId, int roleId);
        Task<(bool Success, string Message)> RemoveRoleAsync(long userId, int roleId);
    }
}
