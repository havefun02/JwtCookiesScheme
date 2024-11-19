using System.Security.Claims;
using JwtCookiesScheme.Entities;

namespace JwtCookiesScheme.Interfaces
{
    public class RefreshTokenResult
    {
        public required string token { get; set; }
        public required DateTime expiredAt { get; set; }
    }
    public interface IJwtService 
    {
        string GenerateAccessToken(IDictionary<string,object> claims);
        ClaimsPrincipal ValidateAccessToken(string token);
        RefreshTokenResult GenerateRefreshToken();
    }
}
