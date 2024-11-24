using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Dtos;
using JwtCookiesScheme.Types;
using Microsoft.AspNetCore.Authentication;
using JwtCookiesScheme.Services;
using JwtCookiesScheme.ViewModels;

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
        public IActionResult Forbidden()
        {
            return PartialView("~/Views/Shared/_Forbidden.cshtml");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest loginDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Alert"] =new ErrorMessageViewModel() { Message = "Invalid Username or Password", AlertType = AlertType.info.ToString() };
                return View(loginDto);
            }
            try
            {

                var result=await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password,false,true);
                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                        TempData["Alert"] = new ErrorMessageViewModel() { Message = "Your are locked out, please try after 5 minutes", AlertType = AlertType.info.ToString() };
                    else
                        TempData["Alert"] = new ErrorMessageViewModel() { Message = "Please try again", AlertType = AlertType.info.ToString() };
                    return View(loginDto);

                }
                else
                {
                    return RedirectToAction("Profile", "User");
                }
            }
            catch (Exception ex)
            {
                TempData["Alert"] = new ErrorMessageViewModel() { Message = ex.Message, AlertType = AlertType.warning.ToString() };

                return View(loginDto);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequest registerDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Alert"] = new ErrorMessageViewModel() { Message = "Invalid data input.", AlertType = AlertType.info.ToString() };
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
                    TempData["Alert"] = new ErrorMessageViewModel() { Message = registerResult.ErrorMessage!, AlertType = AlertType.info.ToString() };
                    return View(registerDto);
                }
            }
            catch (Exception ex)
            {
                TempData["Alert"] = new ErrorMessageViewModel() { Message = ex.Message, AlertType = AlertType.warning.ToString() };
                return View(registerDto);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordDto)
        {
            if (!ModelState.IsValid) { 
                TempData["Alert"] = new ErrorMessageViewModel() { Message = "Invalid data input.", AlertType = AlertType.warning.ToString() };
                return View(changePasswordDto);

            }
            try
            {
                var changePasswordResult = await _authService.ChangePasswordAsync(changePasswordDto);
                if (changePasswordResult.ChangePasswordResult == Result.Success) { 
                    TempData["Alert"] = new ErrorMessageViewModel() { Message = changePasswordResult.ChangePasswordSuccess, AlertType = AlertType.info.ToString() };
                    return RedirectToAction("Login", "Auth"); 
                }
                else
                {
                    TempData["Alert"] = new ErrorMessageViewModel() { Message = changePasswordResult.ChangePasswordErrorMessage, AlertType = AlertType.info.ToString() };
                    return View(changePasswordDto);
                }
            }
            catch (Exception ex)
            {
                TempData["Alert"] = new ErrorMessageViewModel() { Message = ex.Message, AlertType = AlertType.warning.ToString() };
                return View(changePasswordDto);
            }
        }
        
    }
}