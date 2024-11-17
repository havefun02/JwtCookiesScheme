using CRUDFramework;
using JwtCookiesScheme.Dtos;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JwtCookiesScheme.Services
{
    public class TokenService : ITokenService<RefreshToken>
    {
        private readonly IRepository<RefreshToken, DatabaseContext> _tokenRepo;
        private readonly IJwtService<User> _jwtService;
        private readonly UserManager<User> _userManager;

        public TokenService(UserManager<User> userManager,IRepository<RefreshToken, DatabaseContext> tokenRepo, IJwtService<User> jwtService)
        {
            _userManager = userManager;
            _tokenRepo = tokenRepo;
            _jwtService = jwtService;
        }

        public ClaimsPrincipal ValidateAccessToken(string token)
        {
            return this._jwtService.ValidateAccessToken(token);
        }
        public  async Task<PairTokenResult> GenerateTokensAsync(User user)
        {
            try
            {
                var userRole = await _userManager.GetRolesAsync(user);
                var accessToken = this._jwtService.GenerateAccessToken(user, userRole);
                var refreshToken = this._jwtService.GenerateRefreshToken();
                var dbContext = _tokenRepo.GetDbSet();
                var isExisted = await dbContext.SingleOrDefaultAsync(t => t.UserId == user.Id);
                if (isExisted != null)
                {
                    isExisted.Value = refreshToken.token;
                    isExisted.TokenExpiredAt = refreshToken.expiredAt;
                    await _tokenRepo.Update(isExisted);
                }
                else
                {
                    await _tokenRepo.CreateAsync(new RefreshToken() { TokenExpiredAt = refreshToken.expiredAt,Value=refreshToken.token,UserId=user.Id ,isInvoked=false,Id=Guid.NewGuid().ToString()});
                }
                return new PairTokenResult() { AccessToken = accessToken, RefreshToken = refreshToken.token };
            }
            catch (Exception) {
                throw;
            }
        }

        public async Task<RefreshToken> GetTokenInfoAsync(string token)
        {
            try
            {
                var dbContext = _tokenRepo.GetDbSet();
                var tokenResult = await dbContext.SingleOrDefaultAsync(t => t.Value == token);
                if (tokenResult == null) throw new ArgumentException("Can not find token");
                return tokenResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<RefreshToken> UpdateTokenAsync(RefreshToken token)
        {
            try
            {
                var newResetToken = _jwtService.GenerateRefreshToken();
                token.Value = newResetToken.token;
                token.TokenExpiredAt = newResetToken.expiredAt;
                var updatedToken = await _tokenRepo.Update(token);
                return updatedToken;
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to update token", ex);
            }
        }
        public async Task CancelToken(User user)
        {
            try
            {
                var context= _tokenRepo.GetDbSet();
                var tokenResult = await context.SingleOrDefaultAsync(t=>t.UserId==user.Id);
                if (tokenResult != null)
                {
                    tokenResult.isInvoked = true;
                    await _tokenRepo.Update(tokenResult);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to cancel token", ex);
            }
        }



    }
}
