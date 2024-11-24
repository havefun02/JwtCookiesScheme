using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Security.Claims;

namespace JwtCookiesScheme.Services
{
    public class AppSignInManager : SignInManager<User>
    {
        private readonly IJwtService _jwtService;
        private readonly DatabaseContext _context;
        private readonly IEncryptionService _encryptionService;
        public AppSignInManager(
            AppUserManager userManager, 
            IHttpContextAccessor contextAccessor, 
            IUserClaimsPrincipalFactory<User> claimsFactory, 
            IOptions<IdentityOptions> optionsAccessor, 
            ILogger<SignInManager<User>> logger, 
            IAuthenticationSchemeProvider schemes, 
            IUserConfirmation<User> confirmation,
            IJwtService jwtService,
            IEncryptionService encryptionService,
             DatabaseContext context

            )
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _jwtService = jwtService;
            _encryptionService = encryptionService;
            _context = context;
        }
        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure=true)
        {
            try
            {
                var user = await base.UserManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return SignInResult.Failed;
                }

                var attempt = await base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
                if (attempt.Succeeded)
                {
                    await SignInAsync(user, null);
                }
                return attempt;
            }
            catch (Exception) { 
                return SignInResult.Failed;
            }
        }

        protected IDictionary<string,object> ConvertClaimsPrincipalToDict(ClaimsPrincipal claimsPrincipal)
        {
            var claimsDictionary = new Dictionary<string, object>();

            foreach (var claim in claimsPrincipal.Claims)
            {
                if (claimsDictionary.ContainsKey(claim.Type))
                {
                    if (claimsDictionary[claim.Type] is List<string> claimList)
                    {
                        claimList.Add(claim.Value);
                    }
                    else
                    {
                        claimsDictionary[claim.Type] = new List<string>
                        {
                            claimsDictionary[claim.Type].ToString(),
                            claim.Value
                        };
                    }
                }
                else
                {
                    claimsDictionary[claim.Type] = claim.Value;
                }
            }

            return claimsDictionary;
        }

        public override async Task SignInAsync(User user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            try
            {
                var claimsPrincipal = await CreateUserPrincipalAsync(user);
                var dictionaryClaims = ConvertClaimsPrincipalToDict(claimsPrincipal);
                var accessToken = _jwtService.GenerateAccessToken(dictionaryClaims);
                var refreshToken = _jwtService.GenerateRefreshToken();
                await StoreRefreshTokenAsync(user, refreshToken);
                var httpContext = Context;
                if (httpContext != null)
                {
                    httpContext.Response.Cookies.Append("accessToken", _encryptionService.EncryptData(accessToken), new CookieOptions
                    {
                        HttpOnly = true,
                        //Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    httpContext.Response.Cookies.Append("refreshToken", _encryptionService.EncryptData(refreshToken.token), new CookieOptions
                    {
                        HttpOnly = true,
                        //Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
                }
                //var additionalClaims = new List<Claim>();
                //await base.SignInWithClaimsAsync(user, authenticationProperties, additionalClaims);
            }
            catch
            {
                throw;
            }
        }

        public virtual async Task StoreRefreshTokenAsync(User user,RefreshTokenResult token)
        {
            if (token == null) { throw new Exception("Failed to generate refresh token."); }
            var checkToken = await _context.RefreshTokens.SingleOrDefaultAsync(t => t.UserId == user.Id);
            if (checkToken==null)
            {
                await _context.RefreshTokens.AddAsync(new RefreshToken
                {
                    isInvoked = false,
                    Id = Guid.NewGuid().ToString(),
                    TokenExpiredAt = token.expiredAt,
                    UserId = user.Id,
                    Value = token.token
                });
            }
            else
            {
                checkToken.Value = token.token;
                checkToken.TokenExpiredAt = token.expiredAt;
                _context.RefreshTokens.Update(checkToken);
            }
            await _context.SaveChangesAsync();
        }

        public virtual async Task<ClaimsPrincipal?> ValidateTokens(string accessToken,string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken)) return null;
                accessToken=_encryptionService.DecryptData(accessToken);
                refreshToken=_encryptionService.DecryptData(refreshToken);
                var checkTokenResult = await _context.RefreshTokens.SingleOrDefaultAsync(t => t.Value == refreshToken);
                if (checkTokenResult == null) { return null; }
                if (checkTokenResult.TokenExpiredAt < DateTime.UtcNow)
                {
                    return null;
                }

                var claimsPrincipal = this._jwtService.ValidateAccessToken(accessToken);
                if (claimsPrincipal == null) return null;
                return claimsPrincipal;
            }
            catch { throw; }
        }
        public virtual async Task<ClaimsPrincipal?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken)) return null;
                refreshToken = _encryptionService.DecryptData(refreshToken);
                var checkTokenResult = await _context.RefreshTokens.SingleOrDefaultAsync(t => t.Value == refreshToken);
                if (checkTokenResult == null) { return null; }
                if (checkTokenResult.TokenExpiredAt < DateTime.UtcNow)
                {
                    return null;
                }
                var user=await this.UserManager.FindByIdAsync(checkTokenResult.UserId);
                if (user== null) return null;
                await this.SignInAsync(user,null);
                return await CreateUserPrincipalAsync(user);

            }
            catch { throw; }
        }



        public override async Task<SignInResult> PasswordSignInAsync(User user, string password,
       bool isPersistent, bool lockoutOnFailure)
        {
            ArgumentNullException.ThrowIfNull(user);
            var attempt = await CheckPasswordSignInAsync(user, password, lockoutOnFailure);
            if (attempt.Succeeded) {
                await this.SignInAsync(user, isPersistent);    
            }
            return attempt;
        }
    }
}
