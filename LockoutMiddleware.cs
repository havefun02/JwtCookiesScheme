using JwtCookiesScheme.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace JwtCookiesScheme
{
    public class LockoutMiddleware
    {
        private readonly RequestDelegate _next;

        public LockoutMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var _lockoutService = scope.ServiceProvider.GetRequiredService<ILockoutService>();

                if (context.Request.Path.HasValue && context.Request.Path.Value.Equals("/auth/login", StringComparison.OrdinalIgnoreCase) && context.Request.Method == HttpMethods.Post)
                {
                    Console.WriteLine("Lockout check");
                    var sessionId = context.Request.Cookies["ssid"];
                    if (string.IsNullOrEmpty(sessionId))
                    {
                        sessionId = Guid.NewGuid().ToString(); // Generate new session ID if not present
                        context.Response.Cookies.Append("ssid", sessionId);
                    }

                    if (await _lockoutService.IsLockedOutAsync(sessionId))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden; // Forbidden
                        await context.Response.WriteAsync("Your session is locked. Try again later.");
                        return;
                    }
                    var originalBodyStream = context.Response.Body;
                    using (var responseBody = new System.IO.MemoryStream())
                    {
                        context.Response.Body = responseBody;
                        await _next(context);
                        var isLoggedIn = context.Request.Cookies.ContainsKey("isLogged") &&
                 context.Request.Cookies["isLoggedIn"]!.Equals("Yes", StringComparison.OrdinalIgnoreCase);


                        if (!isLoggedIn)
                        {
                            await _lockoutService.RegisterFailedAttemptAsync(sessionId);
                        }
                        else
                        {
                            await _lockoutService.ResetFailedAttemptsAsync(sessionId);
                        }

                        context.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
                else
                    await _next(context);
            }
        }
    }
}
