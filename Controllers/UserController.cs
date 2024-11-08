using AutoMapper;
using JwtCookiesScheme.Entities;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Services;
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
        //[Authorize(Policy ="AdminOnly")]
        [Authorize(Policy ="ExecuteOnly")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            ViewData["Title"] = "Profile";
            var userData =await _userService.GetProfile(userId);
            var userView = _mapper.Map<UserViewModel>(userData);

            return View(userView);
        }

        [Authorize(Policy = "AdminOnly")]

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
                HttpContext.Response.Cookies.Delete("isLogged");
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
