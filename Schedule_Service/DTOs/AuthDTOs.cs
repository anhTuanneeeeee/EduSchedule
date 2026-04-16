using System.ComponentModel.DataAnnotations;

namespace Schedule_Service.DTOs;

public class LoginRequest
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

public class RegisterRequest
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string FullName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    [Required]
    public string RoleCode { get; set; } = null!; // TEACHER, STUDENT, etc.
}

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public DateTime Expiry { get; set; }
    public long UserId { get; set; }
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public List<string> Roles { get; set; } = new List<string>();
}
