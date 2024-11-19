
using CRUDFramework;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Dtos;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using JwtCookiesScheme.Types;
using Serilog;

namespace JwtCookiesScheme.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppUserManager _userManager;
        private readonly AppSignInManager _signInManager;


        public AuthService(
            AppUserManager userManager,
            AppSignInManager signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = new User { UserName = request.UserName, Email = request.UserEmail ,PhoneNumber=request.UserPhone,PhoneNumberConfirmed=false,Id=Guid.NewGuid().ToString() };
            var result = await _userManager.CreateAsync(user,request.UserPassword);
            if (!result.Succeeded)
            {
                return new RegisterResponse { RegisterResult = Result.Fail, ErrorMessage="Register fail" };
            }
            var roleResult = await _userManager.AddToRoleAsync(user, RoleEnum.GUEST.ToString());
            if (!roleResult.Succeeded)
            {

                return new RegisterResponse { RegisterResult = Result.Fail, ErrorMessage = "Failed to create role during register" };
            }
            return new RegisterResponse { RegisterResult =Result.Success, SuccessMessage = "Register successfully" };

        }
      
        public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    return new ChangePasswordResponse { ChangePasswordResult = Result.Fail, ChangePasswordErrorMessage = "Invalid username or password." };
                }

                if (request.NewPassword != request.ConfirmPassword)
                {
                    return new ChangePasswordResponse { ChangePasswordResult = Result.Fail, ChangePasswordErrorMessage = "Confirm password is not correct." };
                }
                var updateResult = _userManager.ChangePasswordAsync(user,request.Password,request.NewPassword);

                if (updateResult.IsCompletedSuccessfully){
                    return new ChangePasswordResponse { ChangePasswordResult = Result.Success, ChangePasswordSuccess = "Change password successfully" };
                }
                return new ChangePasswordResponse { ChangePasswordResult = Result.Fail, ChangePasswordErrorMessage = "Internal server error." };

            }
            catch { throw; }
        }
    }
}
