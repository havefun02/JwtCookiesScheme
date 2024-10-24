using System.ComponentModel.DataAnnotations;

namespace JwtCookiesScheme.Interfaces
{
    public class LoginDto
    {
        [EmailAddress(ErrorMessage = "Please input an email address")]
        public string UserEmail { get; set; } = string.Empty; // Email of the user
        public string UserPassword { get; set; } = string.Empty; // Password of the user
    }
    public class ChangePasswordDto
    {
        public string UserEmail { get; set; } = string.Empty; // Current password of the user
        public string CurrentPassword { get; set; } = string.Empty; // Current password of the user
        public string NewPassword { get; set; } = string.Empty; // New password to be set
    }
    public class RegisterDto
    {
        public string UserName { get; set; } = string.Empty; // Username of the user
        [EmailAddress(ErrorMessage = "Please enter an email")]
        public string UserEmail { get; set; } = string.Empty; // Email of the user
        public string UserPassword { get; set; } = string.Empty; // Password of the user
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string UserPhone { get; set; } = string.Empty; // Phone number of the user (optional)

    }
    public interface IAuthService<Entity>
    {
        public Task<(string, string)> Login(LoginDto user);
        public Task<bool> Register(RegisterDto user);
        public Task<bool> ChangePassword(ChangePasswordDto dto);
    }
}
