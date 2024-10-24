using AutoMapper;
using JwtCookiesScheme.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JwtCookiesScheme.Interfaces;

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
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var (access, refresh) = await _authService.Login(loginDto);
                if (access == null || refresh == null) return NotFound("Please login again!");
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
                var returnUrl = Request.Query["returnUrl"].ToString();
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Ok(new { RedirectUrl = returnUrl });

                }
                return Ok(new { RedirectUrl = "/User/Profile" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Form data is not correct");
            }
            try
            {
                var registerResult = await _authService.Register(registerDto);
                if (registerResult)
                {
                    return Ok(new { RedirectUrl = "/auth/login" });
                }
                else
                {
                    return BadRequest("Register failed");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var changePasswordResult = await _authService.ChangePassword(changePasswordDto);
                if (changePasswordResult) return Ok(new { redirectUrl = "/auth/login" });
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}