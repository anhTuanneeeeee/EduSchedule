using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Schedule_Repository.IRepository;
using Schedule_Repository.Models;
using Schedule_Service.DTOs;
using Schedule_Service.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Schedule_Service.Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, ITeacherRepository teacherRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _teacherRepository = teacherRepository;
        _config = config;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        var roles = await _userRepository.GetUserRolesAsync(user.UserId);
        var token = GenerateJwtToken(user, roles);

        return new AuthResponse
        {
            Token = token.token,
            Expiry = token.expiry,
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Roles = roles
        };
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        // Check if user exists
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null) return false;

        var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
        if (existingEmail != null) return false;

        // Create User
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true
        };

        var createdUser = await _userRepository.CreateAsync(user);

        // Assign Role
        await _userRepository.AddUserRoleAsync(createdUser.UserId, request.RoleCode);

        // If role is TEACHER, create Teacher profile automatically or wait for manual step?
        // Usually better to create a skeleton profile.
        if (request.RoleCode.ToUpper() == "TEACHER")
        {
            await _teacherRepository.CreateTeacherProfileAsync(new Teacher
            {
                UserId = createdUser.UserId,
                TeacherCode = "TCH" + createdUser.UserId, // Placeholder code
                IsAvailableForProjectReview = true,
                MaxAssignmentsPerDay = 4 // Default from requirements
            });
        }

        return true;
    }

    private (string token, DateTime expiry) GenerateJwtToken(User user, List<string> roles)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expiry = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryInMinutes"]!));
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiry,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return (tokenHandler.WriteToken(token), expiry);
    }
}
