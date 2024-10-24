using CRUDFramework;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JwtCookiesScheme.Services
{
    public class TokenService : ITokenService<ResetToken>
    {
        private readonly IRepository<ResetToken, DatabaseContext> _tokenRepo;
        private readonly IJwtService<User> _jwtService;

        public TokenService(IRepository<ResetToken, DatabaseContext> tokenRepo, IJwtService<User> jwtService)
        {
            _tokenRepo = tokenRepo;
            _jwtService = jwtService;
        }

        public async Task<ResetToken> GetTokenInfo(string token)
        {
            try
            {
                var dbContext = _tokenRepo.GetDbSet();
                var tokenResult = await dbContext.Include(t => t.User).SingleOrDefaultAsync(t => t.TokenSerect == token);
                if (tokenResult == null) throw new ArgumentException("Can not find token");
                return tokenResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ResetToken> UpdateTokenAsync(ResetToken token)
        {
            try
            {
                var newResetToken = _jwtService.GenerateRefreshToken();
                token.TokenSerect = newResetToken.token;
                token.TokenExpiredAt = newResetToken.expiredAt;
                var updatedToken = await _tokenRepo.Update(token);
                return updatedToken;
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to update token", ex);
            }
        }

    }
}
