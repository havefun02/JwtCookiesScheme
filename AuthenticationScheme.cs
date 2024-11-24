using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.Encodings.Web;
namespace JwtCookiesScheme
{
    public class AuthenticationAppScheme : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AppUserManager _userManager;
        private readonly AppSignInManager _appSignInManager;
        private readonly IOptionsMonitor<CookieAuthenticationOptions> _cookieOptions;


        private string accessToken;
        private string refreshToken;

        public AuthenticationAppScheme(
            IOptionsMonitor<CookieAuthenticationOptions> cookieOptions,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IEncryptionService encryptionService,
            AppUserManager userManager,
            AppSignInManager appSignInManager

            )
            : base(options, logger, encoder)
        {
            _cookieOptions = cookieOptions;
            _appSignInManager = appSignInManager;
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
                var result= await this._appSignInManager.RefreshTokenAsync(refreshToken);  
                if (result==null)
                    return AuthenticateResult.Fail(new Exception("Please login again"));

                var ticket = new AuthenticationTicket(result, "JWT-COOKIES-SCHEME");
                return AuthenticateResult.Success(ticket);
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
            var cookieAuthOptions = _cookieOptions.Get(CookieAuthenticationDefaults.AuthenticationScheme);
            var loginPath = cookieAuthOptions.LoginPath;
            Context.Response.Redirect(loginPath);
            return Task.CompletedTask;
        }
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            var cookieAuthOptions = _cookieOptions.Get(CookieAuthenticationDefaults.AuthenticationScheme);
            var forbiddenPath = cookieAuthOptions.AccessDeniedPath;
            Context.Response.Redirect(forbiddenPath);
            return Task.CompletedTask;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            var isLoginEndpoint = Context.Request.Path.StartsWithSegments("/auth/login");
            refreshToken = Context.Request.Cookies["refreshToken"];
            accessToken = Context.Request.Cookies["accessToken"];
            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            {
                return AuthenticateResult.Fail(new Exception("Unauthorized."));
            }
            try
            {
                var result = await _appSignInManager.ValidateTokens(accessToken, refreshToken);
                if (result==null) return AuthenticateResult.Fail(new Exception("Unauthorized."));
                var ticket = new AuthenticationTicket(result, "JWT-COOKIES-SCHEME");
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

                return AuthenticateResult.Fail(new Exception("Your refresh token is expired. Please login again."));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }
        }
    }
}
