using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.Encodings.Web;

namespace JwtCookiesScheme
{
    public class AuthenticationScheme:AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IJwtService<User> _jwtService;
        private readonly ITokenService<ResetToken> _tokenService;

        public AuthenticationScheme(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IJwtService<User> jwtService,
            ITokenService<ResetToken> tokenService
            )
            : base(options, logger, encoder)
        {
            _jwtService = jwtService;
            _tokenService = tokenService;
        }

        bool NeedAuthorize(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null) return false;

            var controllerDescriptor = endpoint.Metadata
                .GetMetadata<ControllerActionDescriptor>()
                ?.ControllerTypeInfo;

            var hasAuthorizeOnController = controllerDescriptor?
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Any() ?? false;
            return hasAuthorizeOnController;
        }
        private async Task<AuthenticateResult> HandleExpiredTokenAsync()
        {
            var refreshToken = Context.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return AuthenticateResult.Fail("Cannot find your refresh token");
            }

            try
            {
                var tokenInfo = await _tokenService.GetTokenInfo(refreshToken);
                if (tokenInfo == null || tokenInfo.TokenExpiredAt < DateTime.UtcNow)
                {
                    return AuthenticateResult.Fail("Invalid refresh token");
                }

                var newAccessToken = _jwtService.GenerateAccessToken(tokenInfo.User!);
                if (!string.IsNullOrEmpty(newAccessToken))
                {
                    var newRefreshToken = await _tokenService.UpdateTokenAsync(tokenInfo);
                    Context.Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                    });

                    Context.Response.Cookies.Append("refreshToken", newRefreshToken.TokenSerect!, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                    });
                    var principal = _jwtService.ValidateAccessToken(newAccessToken);
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
            Context.Response.Redirect($"/auth/login");
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
            
            if (NeedAuthorize(Context) || isLoginEndpoint)
            {
                var token = Context.Request.Cookies["accessToken"];
                if (string.IsNullOrEmpty(token))
                {
                    Context.Response.Cookies.Delete("isLogged");
                    return AuthenticateResult.Fail("No token provided");
                }
                try
                {
                    var principal = _jwtService.ValidateAccessToken(token);
                    var ticket = new AuthenticationTicket(principal, "JWT-COOKIES-SCHEME");
                    if (isLoginEndpoint)
                    {
                        Context.Response.Redirect("/User/Profile");
                    }
                    Context.Response.Cookies.Append("isLogged", "Yes", new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                    });
                    return AuthenticateResult.Success(ticket);
                }
                catch (SecurityTokenExpiredException)
                {
                    var verifyExpired=await HandleExpiredTokenAsync();
                    if (verifyExpired.Succeeded) {
                        if (isLoginEndpoint)
                        {
                            Context.Response.Redirect("/User/Profile");
                        }
                        Context.Response.Cookies.Append("isLogged", "Yes", new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                        });
                        return verifyExpired;
                    }
                    Context.Response.Cookies.Delete("isLogged");
                    return verifyExpired;

                }
                catch (Exception ex)
                {
                    Context.Response.Cookies.Delete("isLogged");
                    return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
                }
            }

            else
            {
                Context.Response.Cookies.Delete("isLogged");
                return AuthenticateResult.NoResult();
            }
        }
    }
}
