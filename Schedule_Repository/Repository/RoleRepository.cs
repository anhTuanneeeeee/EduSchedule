using Microsoft.EntityFrameworkCore;
using Schedule_Repository.IRepository;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public RoleRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int roleId)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<bool> RoleExistsAsync(int roleId)
        {
            return await _context.Roles
                .AnyAsync(r => r.RoleId == roleId);
        }
    }
}
