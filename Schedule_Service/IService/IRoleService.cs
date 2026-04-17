using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.IService
{
    public interface IRoleService
    {
        Task<List<RoleResponseDto>> GetAllAsync();
        Task<(bool Success, string Message, RoleResponseDto? Data)> CreateAsync(CreateRoleRequestDto request);
    }
}

