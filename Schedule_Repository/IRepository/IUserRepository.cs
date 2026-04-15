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
}
