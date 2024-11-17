using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JwtCookiesScheme;

namespace Microsoft.Extensions.DependencyInjection;
public static class IdentityServiceCollectionExtensions
{
    public static IdentityBuilder AddAppIdentity<TUser,TRole>(this IServiceCollection services)
        where TUser : class
        where TRole : class
        => services.AddAppIdentity<TUser,TRole>(o => { });
    public static IdentityBuilder AddAppIdentity<TUser,TRole>(this IServiceCollection services, Action<IdentityOptions> setupAction) 
        where TUser : class
        where TRole : class

    {

        services.AddOptions().AddLogging();

        services.AddHttpContextAccessor();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "JWT-COOKIES-SCHEME";
            options.DefaultSignInScheme = "JWT-COOKIES-SCHEME";
            options.DefaultChallengeScheme = "JWT-COOKIES-SCHEME";
        })
 .AddScheme<AuthenticationSchemeOptions, AuthenticationAppScheme>(
     "JWT-COOKIES-SCHEME",
     options => { });


        // Services used by identity
        services.TryAddScoped<IUserValidator<TUser>, UserValidator<TUser>>();
        services.TryAddScoped<IPasswordValidator<TUser>, PasswordValidator<TUser>>();
        services.TryAddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
        services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
        services.TryAddScoped<IUserConfirmation<TUser>, DefaultUserConfirmation<TUser>>();
        services.TryAddScoped<IRoleValidator<TRole>, RoleValidator<TRole>>();
        // No interface for the error describer so we can add errors without rev'ing the interface
        services.TryAddScoped<IdentityErrorDescriber>();
        services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<TUser>>();
        services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<TUser>>();
        services.TryAddScoped<IUserClaimsPrincipalFactory<TUser>, UserClaimsPrincipalFactory<TUser, TRole>>();
        services.TryAddScoped<IUserConfirmation<TUser>, DefaultUserConfirmation<TUser>>();

        services.TryAddScoped<UserManager<TUser>>();
        services.TryAddScoped<SignInManager<TUser>>();
        services.TryAddScoped<RoleManager<TRole>>();

        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        return new IdentityBuilder(typeof(TUser),typeof(TRole), services);
    }
}