using System.ComponentModel.DataAnnotations;

namespace JwtCookiesScheme.Dtos
{
    public class LoginDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please input an email address")]
        public string? UserEmail { get; set; } 
        public string? UserPassword { get; set; } 
    }
    public class ChangePasswordDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]

        public string? UserEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public required string CurrentPassword { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

    }
    public class RegisterDto
    {
        public required string  UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter an email")]
        public required string UserEmail { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public required string UserPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("UserPassword", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

        public string UserPhone { get; set; } = string.Empty; 

    }
}
