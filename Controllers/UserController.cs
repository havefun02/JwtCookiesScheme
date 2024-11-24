using AutoMapper;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Services;
using JwtCookiesScheme.Types;
using JwtCookiesScheme.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtCookiesScheme.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService<User> _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService<User> userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value;
                var claims = User.Claims;
                foreach (var claim in claims)
                {
                    Console.WriteLine(claim.Type + " " + claim.Value);
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    TempData["Alert"] = new ErrorMessageViewModel() {Message= "Some errors occured. Please login again." ,AlertType=AlertType.danger.ToString()};
                    HttpContext.Response.Cookies.Delete("accessToken");
                    HttpContext.Response.Cookies.Delete("refreshToken");
                    return RedirectToAction("Login", "Auth");
                }
                ViewData["Title"] = "Profile";
                var userData = await _userService.GetProfile(userId);
                var userView = _mapper.Map<UserViewModel>(userData);
                return View(userView);
            }
            catch (Exception ex)
            {
                TempData["Alert"] = new ErrorMessageViewModel() { Message = ex.Message, AlertType = AlertType.warning.ToString() };
                return View();
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            ViewData["Title"] = "Index";
            var userData =await _userService.GetAllUser();
            var userView = _mapper.Map<List<UserViewModel>>(userData);

            return View(userView);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound("User ID not found in claims.");
                }
                HttpContext.Response.Cookies.Delete("accessToken");
                HttpContext.Response.Cookies.Delete("refreshToken");
                return RedirectToAction("Login","Auth");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
