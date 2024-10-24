using System.Security.Claims;
using JwtCookiesScheme.Entities;

namespace JwtCookiesScheme.Interfaces
{
    public class RefreshTokenResult
    {
        public required string token { get; set; }
        public required DateTime expiredAt { get; set; }
    }
    public interface IJwtService<TUser> where TUser : UserBase
    {
        string GenerateAccessToken(TUser user);
        ClaimsPrincipal ValidateAccessToken(string token);
        RefreshTokenResult GenerateRefreshToken();
    }
}
