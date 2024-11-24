using JwtCookiesScheme.Dtos;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Types;
using JwtCookiesScheme.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Reflection.Metadata.Ecma335;

namespace JwtCookiesScheme.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppUserManager _appUserManager;
        private readonly  RoleManager<Role> _roleManager;

        public AdminService(AppUserManager appUserManager,RoleManager<Role> roleManager) {
            _roleManager = roleManager;
            _appUserManager=appUserManager;
        }
        public async Task DeleteDataUser(string adminId, string userId)
        {
            try
            {
                var admin = await _appUserManager.FindByIdAsync(adminId);
                if (admin == null) { throw new Exception("Admin does not exist."); }
                var user = await _appUserManager.Users.Include(r => r.UserRoles).ThenInclude(t=>t.Role).FirstOrDefaultAsync(t => t.Id == userId);
                if (user == null) { throw new Exception("User does not exist"); }
                var userRolesName = user.UserRoles.Select(t => t.Role.Name).ToList();
                if (userRolesName.Contains(RoleEnum.ADMIN.ToString()) || userRolesName.Contains(RoleEnum.OWNER.ToString()))
                    throw new Exception("Can not delete admin or owner");
                var deleteResult=await _appUserManager.DeleteAsync(user);
                if (!deleteResult.Succeeded) {
                    throw new Exception("Failed to delete user");
                }
            }
            catch
            {
                throw;
            }
        }


        public async Task<IEnumerable<User>> GetDashBoard(string adminId)
        {
            try
            {
                var admin = await _appUserManager.FindByIdAsync(adminId);
                if (admin == null) { throw new Exception("Admin does not exist."); }
                var users =await _appUserManager.Users.Where(u => u.Id != adminId).Include(t=>t.UserRoles).ThenInclude(u=>u.Role).ToListAsync();
                return users;
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateUser(string adminId, EditRequest userInfo)
        {
            try {
                var admin = await _appUserManager.FindByIdAsync(adminId);
                if (admin == null) { throw new Exception("Admin does not exist."); }
                var user=await _appUserManager.Users.Include(r=>r.UserRoles).FirstOrDefaultAsync(t=>t.Email==userInfo.UserEmail);
                if (user == null) { throw new Exception("User does not exist"); }
                user.PhoneNumber = userInfo.UserPhone;
                user.PhoneNumberConfirmed = false;
                user.LastName= userInfo.UserLastName;
                user.FirstName= userInfo.UserFirstName;
                var userRoles=new List<UserRole>();
                userInfo.UserRoles.ForEach(async role =>
                {
                    var roleResult = await _roleManager.FindByNameAsync(role);
                    if (!user.UserRoles.Contains(new UserRole { UserId=user.Id,RoleId=roleResult.Id}))
                        userRoles.Add(new UserRole { RoleId = roleResult.Id,UserId=user.Id });
                });
                user.UserRoles = userRoles;
                await _appUserManager.UpdateAsync(user);
            }
            catch {
                throw;
            }
        }
        public async Task<User> GetUserData(string adminId,string userId)
        {
            var admin = await _appUserManager.FindByIdAsync(adminId);
            if (admin == null) { throw new Exception("Admin does not exist."); }
            var user = await _appUserManager.FindByIdAsync(userId);
            if (user == null) { throw new Exception("User does not exist"); }

            return user;

        }
    }
}
