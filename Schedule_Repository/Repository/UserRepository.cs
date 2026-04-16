using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using Schedule_Repository.IRepository;

namespace Schedule_Repository.Repository;

public class UserRepository : IUserRepository
{
    private readonly ScheduleForTeacherContext _context;

    public UserRepository(ScheduleForTeacherContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task AddUserRoleAsync(long userId, string roleCode)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleCode == roleCode);
        if (role != null)
        {
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = role.RoleId
            };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<string>> GetUserRolesAsync(long userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.RoleCode)
            .ToListAsync();
    }

    public async Task<Teacher?> GetTeacherByUserIdAsync(long userId)
    {
        return await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetByIdWithRolesAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<List<Role>> GetRolesByUserIdAsync(int userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<User>> GetUsersByRoleNameAsync(string roleName)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == roleName))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> UserExistsAsync(int userId)
    {
        return await _context.Users.AnyAsync(u => u.UserId == userId);
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
    {
        return await _context.Users.AnyAsync(u =>
            u.Email == email &&
            (!excludeUserId.HasValue || u.UserId != excludeUserId.Value));
    }

    public async Task<bool> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AssignRoleAsync(int userId, int roleId)
    {
        bool existed = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (existed)
            return false;

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        _context.UserRoles.Add(userRole);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveRoleAsync(int userId, int roleId)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole == null)
            return false;

        _context.UserRoles.Remove(userRole);
        return await _context.SaveChangesAsync() > 0;
    }
}
