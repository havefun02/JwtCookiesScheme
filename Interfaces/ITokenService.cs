using JwtCookiesScheme.Dtos;
using JwtCookiesScheme.Entities;
using System.Security.Claims;

namespace JwtCookiesScheme.Interfaces
{
    public interface ITokenService<T>
    {
        Task<T> GetTokenInfoAsync(string token);
        Task<T> UpdateTokenAsync(T token);
        Task<PairTokenResult> GenerateTokensAsync(User user);
        Task CancelToken(User user);
        ClaimsPrincipal ValidateAccessToken(string token);


    }
}
