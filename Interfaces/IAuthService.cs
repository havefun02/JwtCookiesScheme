using System.ComponentModel.DataAnnotations;
using JwtCookiesScheme.Dtos;
namespace JwtCookiesScheme.Interfaces
{
 
    public interface IAuthService<Entity>
    {
        public Task<(string, string)> Login(LoginDto user);
        public Task<bool> Register(RegisterDto user);
        public Task<bool> ChangePassword(ChangePasswordDto dto);
    }
}
