﻿using System.ComponentModel.DataAnnotations;
using JwtCookiesScheme.Dtos;
namespace JwtCookiesScheme.Interfaces
{
 
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request);
    }
}
