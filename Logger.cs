using System.Diagnostics;
using Serilog;
namespace JwtCookiesScheme
{
    
        public class Logger
        {
            private readonly RequestDelegate _next;

            public Logger(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                var watch = Stopwatch.StartNew();
                Log.Information("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
                await _next(context);
                watch.Stop();
                var elapsedMilliseconds = watch.ElapsedMilliseconds;

                Log.Information("Finished handling request: {Method} {Path} with status code {StatusCode} in {ElapsedMilliseconds}ms",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsedMilliseconds);

            }
    }
}
