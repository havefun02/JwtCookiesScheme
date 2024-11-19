using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Dtos;
using JwtCookiesScheme.Types;
using Microsoft.AspNetCore.Authentication;
using JwtCookiesScheme.Services;

namespace JwtCookiesScheme.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppSignInManager _signInManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper; 
        private readonly IEncryptionService _encryptionService;

        public AuthController(IEncryptionService encryptionService, IAuthService authService, IMapper mapper, AppSignInManager signInManager)
        {
            _mapper = mapper;
            _authService = authService;
            _encryptionService = encryptionService;
            _signInManager = signInManager;
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
        public async Task<IActionResult> Login(LoginRequest loginDto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid Username or Password";
                return View(loginDto);
            }
            try
            {

                var result=await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password,false,false);
                if (!result.Succeeded)
                {
                    ViewData["Error"] = "Please try again";
                    return View(loginDto);

                }
                else
                {
                    return RedirectToAction("Profile", "User");
                }
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(loginDto);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequest registerDto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid data input.";
                return View(registerDto);
            }
            try
            {
                var registerResult = await _authService.RegisterAsync(registerDto);
                if (registerResult.RegisterResult==Result.Success)
                {
                    return RedirectToAction("Login","Auth");
                }
                else
                {
                    ViewData["Error"] = "Invalid data input.";
                    return View(registerDto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ViewData["Error"] = ex.Message;
                return View(registerDto);
            }
        }
        [HttpPost("change-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordDto)
        {
            try
            {
                var changePasswordResult = await _authService.ChangePasswordAsync(changePasswordDto);
                if (changePasswordResult.ChangePasswordResult==Result.Success) return RedirectToAction("Login","Auth");
                else return View(changePasswordDto);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(changePasswordDto);
            }
        }
        
    }
}