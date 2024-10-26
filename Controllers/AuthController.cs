using AutoMapper;
using JwtCookiesScheme.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JwtCookiesScheme.Interfaces;
using Microsoft.AspNetCore.Identity;
using JwtCookiesScheme.Dtos;

namespace JwtCookiesScheme.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService<User> _authService;
        private readonly IMapper _mapper;
        public AuthController(IAuthService<User> authService, IMapper mapper)
        {
            _mapper = mapper;
            _authService = authService;
        }
        [HttpGet]
        public IActionResult Login() 
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Reset()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            try
            {

                var (access, refresh) = await _authService.Login(loginDto);
                if (access == null || refresh == null)
                {
                    return View(loginDto);
                }
                HttpContext.Response.Cookies.Append("accessToken", access, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });
                HttpContext.Response.Cookies.Append("refreshToken", refresh, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });
                HttpContext.Response.Cookies.Append("isLogged", "Yes", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });
                return RedirectToAction("Profile", "User");
            }
            catch (Exception ex)
            {
                return View(loginDto);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }
            try
            {
                var registerResult = await _authService.Register(registerDto);
                if (registerResult)
                {
                    return RedirectToAction("Login","Auth");
                }
                else
                {
                    return View(registerDto);
                }
            }
            catch (Exception)
            {
                return View(registerDto);
            }
        }
        [HttpPost("change-password")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var changePasswordResult = await _authService.ChangePassword(changePasswordDto);
                if (changePasswordResult) return RedirectToAction("Login","Auth");
                else return View(changePasswordDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}