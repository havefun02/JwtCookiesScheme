
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
        private readonly DatabaseContext _databaseContext;

        public AuthService(
            AppUserManager userManager,
            AppSignInManager signInManager,
            DatabaseContext databaseContext

        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _databaseContext = databaseContext;
        }
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            using var transaction = await _databaseContext.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    UserName = request.UserName,
                    Email = request.UserEmail,
                    PhoneNumber = request.UserPhone,
                    PhoneNumberConfirmed = false,
                    Id = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, request.UserPassword);
                if (!result.Succeeded)
                {
                    return new RegisterResponse
                    {
                        RegisterResult = Result.Fail,
                        ErrorMessage = "Register failed: " + string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }

                var roleResult = await _userManager.AddToRoleAsync(user, RoleEnum.GUEST.ToString());
                if (!roleResult.Succeeded)
                {
                    throw new Exception("Role assignment failed: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }

                await transaction.CommitAsync();

                return new RegisterResponse
                {
                    RegisterResult = Result.Success,
                    SuccessMessage = "Register successfully"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return new RegisterResponse
                {
                    RegisterResult = Result.Fail,
                    ErrorMessage = "An error occurred: " + ex.Message
                };
            }
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
