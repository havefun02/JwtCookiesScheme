
using CRUDFramework;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Dtos;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace JwtCookiesScheme.Services
{
    public class AuthService : IAuthService<User>
    {
        private readonly IRepository<User, DatabaseContext> _userRepository;
        private readonly IRepository<Role, DatabaseContext> _roleRepository;
        private readonly IJwtService<User> _jwtService;

        public AuthService(IRepository<User, DatabaseContext> userRepository, IRepository<Role, DatabaseContext> roleRepository, IJwtService<User> jwtService)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
        }
        public async Task<(string, string)> Login(LoginDto dto)
        {
            var userContext = _userRepository.GetDbSet();


            var user = await userContext.Include(u=>u.UserToken).SingleOrDefaultAsync(u => u.UserEmail == dto.UserEmail); // Assuming this method exists

            if (user == null)
            {
                throw new ArgumentException("Invalid email or password.");
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.UserPassword!, dto.UserPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new ArgumentException("Invalid email or password.");
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            try
            {
                if (user.UserToken != null)
                {
                    user.UserToken.TokenSerect = refreshToken.token!;
                    user.UserToken.TokenExpiredAt = refreshToken.expiredAt;
                    await _userRepository.Update(user);
                }
                else
                {
                    var token=new ResetToken() { TokenId=Guid.NewGuid().ToString(),TokenSerect= refreshToken.token,TokenExpiredAt= refreshToken.expiredAt,UserId=user.UserId };
                    user.UserToken = token;
                    await _userRepository.Update(user);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Internal server error during query user login operation " + ex.Message);
            }
            return (accessToken, refreshToken.token!);
        }

        public async Task<bool> Register(RegisterDto user)
        {
            try
            {
                var userContext = _userRepository.GetDbSet();
                var roleContext = _roleRepository.GetDbSet();

                var existingUser = await userContext.SingleOrDefaultAsync(u => u.UserEmail == user.UserEmail);
                if (existingUser != null)
                {
                    throw new ArgumentException("User with this email already exists.");
                }

                var guestRole = await roleContext.SingleOrDefaultAsync(r => r.RoleName == "Guest");
                if (guestRole == null) throw new Exception("Internal exception");

                var newUser = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = user.UserName,
                    UserEmail = user.UserEmail,
                    UserPhone = user.UserPhone,
                    UserRoleId = guestRole.RoleId,
                };

                var passwordHasher = new PasswordHasher<User>();
                newUser.UserPassword = passwordHasher.HashPassword(newUser, user.UserPassword);

                var userCreated = await _userRepository.CreateAsync(newUser);
                if (userCreated != null)
                    return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                var userContext = _userRepository.GetDbSet();
                var user = await userContext.SingleOrDefaultAsync(u => u.UserEmail == dto.UserEmail);
                if (user == null)
                {
                    throw new ArgumentException("User not found.");
                }

                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.UserPassword!, dto.CurrentPassword);

                if (result == PasswordVerificationResult.Failed)
                {
                    throw new ArgumentException("Current password is incorrect.");
                }

                user.UserPassword = passwordHasher.HashPassword(user, dto.NewPassword);
                var updateResult = await _userRepository.Update(user);
                if (updateResult == null) throw new InvalidOperationException("Internal error while changing password");
                return true;
            }
            catch { throw; }
        }
    }
}
