using Schedule_Repository.IRepository;
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
    }
}
