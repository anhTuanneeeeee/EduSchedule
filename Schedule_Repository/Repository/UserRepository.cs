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

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
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
}
