﻿using AutoMapper;
using CRUDFramework;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Mapper;
using JwtCookiesScheme.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

namespace JwtCookiesScheme
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration=configuration;
        }
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<DatabaseContext>();

            services.AddSingleton<IMapper>(provider =>
            {
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<Mapping>();
                });
                return configuration.CreateMapper();
            });
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            services.AddScoped<IUserService<User>, UserService>();
            services.AddScoped<IAuthService<User>, AuthService>();

            services.AddScoped<IJwtService<User>, JwtService>();
            services.AddScoped<ITokenService<ResetToken>, TokenService>();
            services.AddAuthentication("JWT-COOKIES-SCHEME").AddScheme<AuthenticationSchemeOptions, AuthenticationScheme>("JWT-COOKIES-SCHEME", null);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("OwnerOnly", policy =>
                    policy.RequireRole("Owner"));
            });
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "DEVELOPMENT API", Version = "v1" }); });

            services.AddMvc();
        }
        public void Configure(IApplicationBuilder app,IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "documentation";
            });

            app.UseCors("AllowAll");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;
                var statusCode = response.StatusCode;

                if (statusCode == 404)
                {
                    response.ContentType = "text/html";
                    await response.SendFileAsync("wwwroot/notFoundPage.html");
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}");
            });
        }
    }
}
