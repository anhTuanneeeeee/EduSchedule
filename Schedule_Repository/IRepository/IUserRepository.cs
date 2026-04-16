using Schedule_Repository.Models;

namespace Schedule_Repository.IRepository;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task AddUserRoleAsync(long userId, string roleCode);
    Task<List<string>> GetUserRolesAsync(long userId);
    Task<Teacher?> GetTeacherByUserIdAsync(long userId);
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByIdWithRolesAsync(int userId);
    Task<List<Role>> GetRolesByUserIdAsync(int userId);
    Task<List<User>> GetUsersByRoleNameAsync(string roleName);

    Task<bool> UserExistsAsync(int userId);
    Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null);

    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(User user);

    Task<bool> AssignRoleAsync(int userId, int roleId);
    Task<bool> RemoveRoleAsync(int userId, int roleId);
}
