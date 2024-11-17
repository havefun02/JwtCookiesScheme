using AutoMapper;
using CRUDFramework;
using FluentValidation;
using FluentValidation.AspNetCore;
using JwtCookiesScheme.Dtos;
using JwtCookiesScheme.Dtos.Validation;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Mapper;
using JwtCookiesScheme.Policies;
using JwtCookiesScheme.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace JwtCookiesScheme
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration=configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>();
            services.AddAppIdentity<User,Role>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            //services.AddIdentity<User, Role>(options =>
            //{
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(30);
            //    options.Lockout.MaxFailedAccessAttempts = 5;
            //    options.Lockout.AllowedForNewUsers = true;
            //})
            //        .AddEntityFrameworkStores<DatabaseContext>()
            //        .AddDefaultTokenProviders();
          

            services.AddMemoryCache();
            services.AddSingleton<RolePermissionsCacheService>();
            services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler<AdminOnlyRequirement>>();
            services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler<OwnerOnlyRequirement>>();
            services.AddSingleton<IMapper>(provider =>
            {
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<Mapping>();
                });
                return configuration.CreateMapper();
            });
            services.AddDataProtection()
                .SetApplicationName("authApp")
                .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Temp\Keys"))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(30));


            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidation>();
            services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidation>();
            services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordValidation>();
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IUserService<User>, UserService>();
            services.AddScoped<ITokenService<RefreshToken>, TokenService>();
            services.AddScoped<IJwtService<User>, JwtService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IAuthService, AuthService>();
           
            services.AddAuthorization(option =>
            {
                option.AddPolicy("AdminOnly", policy => policy.Requirements.Add(new AdminOnlyRequirement(services.BuildServiceProvider().GetService<RolePermissionsCacheService>())));
                option.AddPolicy("ExecuteOnly", policy => policy.Requirements.Add(new ExecutePermissionOnly(services.BuildServiceProvider().GetService<RolePermissionsCacheService>())));

            });
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "DEVELOPMENT API", Version = "v1" }); });
            services.AddMvc();
        }
        public void Configure(IApplicationBuilder app,IWebHostEnvironment env, IServiceProvider serviceProvider)
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
            app.UseMiddleware<Logger>();
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
