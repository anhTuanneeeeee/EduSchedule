using Schedule_Repository.IRepository;
using Schedule_Service.IService;
using Schedule_Repository.Models;
using Schedule_Repository.Repository;
using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToUserResponse).ToList();
        }
        public async Task<UserResponseDto?> GetByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            return user == null ? null : MapToUserResponse(user);
        }

        public async Task<List<RoleResponseDto>?> GetRolesByUserIdAsync(int userId)
        {
            bool exists = await _userRepository.UserExistsAsync(userId);
            if (!exists)
                return null;

            var roles = await _userRepository.GetRolesByUserIdAsync(userId);

            return roles.Select(r => new RoleResponseDto
            {
                RoleId = (int)r.RoleId,
                RoleName = r.RoleName
            }).ToList();
        }

        public async Task<List<UserResponseDto>> GetUsersByRoleNameAsync(string roleName)
        {
            var users = await _userRepository.GetUsersByRoleNameAsync(roleName);
            return users.Select(MapToUserResponse).ToList();
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int userId, UpdateUserRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "User không tồn tại.");

            bool emailExists = await _userRepository.IsEmailExistsAsync(request.Email, userId);
            if (emailExists)
                return (false, "Email đã tồn tại.");

            user.Username = request.Username;
            user.Email = request.Email;
            user.FullName = request.FullName;
            user.IsActive = (bool)request.IsActive;

            bool result = await _userRepository.UpdateAsync(user);

            return result
                ? (true, "Cập nhật user thành công.")
                : (false, "Cập nhật user thất bại.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "User không tồn tại.");

            bool result = await _userRepository.DeleteAsync(user);

            return result
                ? (true, "Xóa user thành công.")
                : (false, "Xóa user thất bại.");
        }

        public async Task<(bool Success, string Message)> AssignRoleAsync(int userId, int roleId)
        {
            bool userExists = await _userRepository.UserExistsAsync(userId);
            if (!userExists)
                return (false, "User không tồn tại.");

            bool roleExists = await _roleRepository.RoleExistsAsync(roleId);
            if (!roleExists)
                return (false, "Role không tồn tại.");

            bool result = await _userRepository.AssignRoleAsync(userId, roleId);

            return result
                ? (true, "Gán role thành công.")
                : (false, "User đã có role này hoặc gán role thất bại.");
        }

        public async Task<(bool Success, string Message)> RemoveRoleAsync(int userId, int roleId)
        {
            bool userExists = await _userRepository.UserExistsAsync(userId);
            if (!userExists)
                return (false, "User không tồn tại.");

            bool result = await _userRepository.RemoveRoleAsync(userId, roleId);

            return result
                ? (true, "Gỡ role thành công.")
                : (false, "User không có role này hoặc gỡ role thất bại.");
        }
        private UserResponseDto MapToUserResponse(User user)
        {
            return new UserResponseDto
            {
                UserId = (int)user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                Roles = user.UserRoles != null
                    ? user.UserRoles.Select(ur => new RoleResponseDto
                    {
                        RoleId = (int)ur.Role.RoleId,
                        RoleName = ur.Role.RoleName
                    }).ToList()
                    : new List<RoleResponseDto>()
            };
        }
    }
}
