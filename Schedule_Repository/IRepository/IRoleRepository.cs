using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.IRepository
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int roleId);
        Task<Role?> GetByNameAsync(string roleName);
        Task<bool> RoleExistsAsync(int roleId);
        Task<bool> RoleNameExistsAsync(string roleName);
        Task<Role> CreateAsync(Role role);

    }
}
