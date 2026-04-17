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
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleResponseDto>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();

            return roles.Select(r => new RoleResponseDto
            {
                RoleId = (int)r.RoleId,
                RoleName = r.RoleName
            }).ToList();
        }
        public async Task<(bool Success, string Message, RoleResponseDto? Data)> CreateAsync(CreateRoleRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
                return (false, "RoleName không được để trống.", null);

            bool existed = await _roleRepository.RoleNameExistsAsync(request.RoleName.Trim());
            if (existed)
                return (false, "Role đã tồn tại.", null);

            var role = new Role
            {
                RoleName = request.RoleName.Trim()
            };

            var createdRole = await _roleRepository.CreateAsync(role);

            var response = new RoleResponseDto
            {
                RoleId = (int)createdRole.RoleId,
                RoleName = createdRole.RoleName
            };

            return (true, "Tạo role thành công.", response);
        }
    }
}
