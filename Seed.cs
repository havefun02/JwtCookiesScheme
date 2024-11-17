using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Types;
using JwtCookiesScheme;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

public static class Seed
{
    public static async Task SeedingDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        if (dbContext == null || userManager == null || roleManager == null)
        {
            throw new NullReferenceException("Can't resolve necessary services");
        }

        try
        {
            // Seed Roles
            var roles = new[] { "ADMIN", "OWNER", "GUEST", "USER" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new Role { Name = roleName };
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        Log.Warning($"Error creating role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Seed Users
            var users = new[]
            {
            new { Email = "Admin@gmail.com", UserName = "Lapphan", Role = "ADMIN", Id = "AdminId" },
            new { Email = "Owner@gmail.com", UserName = "Lapphan1", Role = "OWNER", Id = "OwnerId" }
        };

            foreach (var userData in users)
            {
                var user = await userManager.FindByEmailAsync(userData.Email);
                if (user == null)
                {
                    user = new User
                    {
                        Id = userData.Id,
                        Email = userData.Email,
                        UserName = userData.UserName,
                        PhoneNumber = "01231231",
                        EmailConfirmed = true
                    };

                    var createUserResult = await userManager.CreateAsync(user, "abcD01@");
                    if (!createUserResult.Succeeded)
                    {
                        Log.Warning($"Error creating user {userData.Email}: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
                        continue;
                    }
                }

                // Assign Role
                if (!await userManager.IsInRoleAsync(user, userData.Role))
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(user, userData.Role);
                    if (!addToRoleResult.Succeeded)
                    {
                        Log.Warning($"Error adding user {userData.Email} to role {userData.Role}: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Seed Role Claims
            var roleClaims = new[]
            {
            new { Role = "ADMIN", Type = "Permission", Value = PermissionEnum.READ.ToString() },
            new { Role = "ADMIN", Type = "Permission", Value = PermissionEnum.WRITE.ToString() },
            new { Role = "OWNER", Type = "Permission", Value = PermissionEnum.READ.ToString() },
            new { Role = "OWNER", Type = "Permission", Value = PermissionEnum.WRITE.ToString() },
            new { Role = "OWNER", Type = "Permission", Value = PermissionEnum.EXECUTE.ToString() }
        };

            foreach (var roleClaim in roleClaims)
            {
                var role = await roleManager.FindByNameAsync(roleClaim.Role);
                if (role == null) continue;

                var claims = await roleManager.GetClaimsAsync(role);
                if (!claims.Any(c => c.Type == roleClaim.Type && c.Value == roleClaim.Value))
                {
                    await roleManager.AddClaimAsync(role, new Claim(roleClaim.Type, roleClaim.Value));
                }
            }
        }
        catch (DbUpdateException dbEx)
        {
            Log.Error($"Database update failed: {dbEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while seeding: {ex.Message}");
            throw;
        }
    }
}