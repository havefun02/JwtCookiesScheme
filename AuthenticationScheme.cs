using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.Encodings.Web;
namespace JwtCookiesScheme
{
    public class AuthenticationAppScheme : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ITokenService<RefreshToken> _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IEncryptionService _encryptionService;
        private string accessToken;
        private string refreshToken;

        public AuthenticationAppScheme(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            ITokenService<RefreshToken> tokenService,
            UrlEncoder encoder,
            IEncryptionService encryptionService,
            UserManager<User> userManager
            )
            : base(options, logger, encoder)
        {
            _encryptionService = encryptionService;
            _tokenService = tokenService;
            _userManager = userManager;
            accessToken = "";
            refreshToken = "";
        }

        private async Task<AuthenticateResult> HandleExpiredTokenAsync()
        {
            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            {
                return AuthenticateResult.Fail(new Exception("Unauthorize"));
            }
            try
            {

                var refreshTokenInfo = await _tokenService.GetTokenInfoAsync(refreshToken);
                if (refreshTokenInfo == null || refreshTokenInfo.TokenExpiredAt < DateTime.UtcNow)
                {
                    return AuthenticateResult.Fail(new Exception("Invalid Token"));
                }
                var user = await _userManager.FindByIdAsync(refreshTokenInfo.UserId);
                if (user == null) {
                    return AuthenticateResult.Fail(new Exception("Invalid refresh token"));
                }

                var tokensResult =await this._tokenService.GenerateTokensAsync(user);
                if (tokensResult != null)
                {
                    Context.Response.Cookies.Append("accessToken", _encryptionService.EncryptData(tokensResult.AccessToken), new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                    });

                    Context.Response.Cookies.Append("refreshToken", _encryptionService.EncryptData(tokensResult.RefreshToken), new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                    });
                    var principal = _tokenService.ValidateAccessToken(tokensResult.AccessToken);
                    var ticket = new AuthenticationTicket(principal, "JWT-COOKIES-SCHEME");
                    return AuthenticateResult.Success(ticket);
                }
                return AuthenticateResult.Fail("Failed to generate new access token");
            }
            catch (ArgumentException ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Error occured while trying to validate reset token: {ex.Message}");
            }
        }
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Context.Response.Redirect("/auth/login");
            return Task.CompletedTask;
        }
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            var isLoginEndpoint = Context.Request.Path.StartsWithSegments("/auth/login");
            refreshToken = Context.Request.Cookies["refreshToken"];
            accessToken = Context.Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            {
                return AuthenticateResult.Fail(new Exception("Unauthorize"));
            }
            try
            {

                refreshToken = _encryptionService.DecryptData(refreshToken);
                accessToken = _encryptionService.DecryptData(accessToken);
                var principal = _tokenService.ValidateAccessToken(accessToken);
                var ticket = new AuthenticationTicket(principal, "JWT-COOKIES-SCHEME");
                if (isLoginEndpoint)
                {
                    Context.Response.Redirect("/User/Profile");
                }

                return AuthenticateResult.Success(ticket);
            }
            catch (SecurityTokenExpiredException)
            {
                var verifyExpired = await HandleExpiredTokenAsync();
                if (verifyExpired.Succeeded)
                {
                    if (isLoginEndpoint)
                    {
                        Context.Response.Redirect("/User/Profile");
                    }
                    return verifyExpired;
                }

                return AuthenticateResult.Fail(new Exception("Failed to refresh token"));
            }

            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }
        }
    }
}
