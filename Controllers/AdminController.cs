using AutoMapper;
using JwtCookiesScheme.Dtos;
using JwtCookiesScheme.Interfaces;
using JwtCookiesScheme.Types;
using JwtCookiesScheme.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtCookiesScheme.Controllers
{
    [Authorize(Policy ="AdminOnly")]
    public class AdminController:Controller
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        public AdminController(IAdminService adminService,IMapper mapper) {
            _adminService = adminService;
            _mapper = mapper;   
        }


        [HttpGet]
        public async Task<IActionResult> DashBoard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            var users = await _adminService.GetDashBoard(userId.Value);
            var userViewModel=_mapper.Map<List<UserItemViewModel>>(users);

            return View(new DashBoardViewModel() { users= userViewModel });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public  async Task<IActionResult> Edit(string id)
        {
            var userId = id;
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier);
            var users = await _adminService.GetUserData(adminId.Value,userId);
            var userViewModel=_mapper.Map<EditRequest>(users);
            return View(userViewModel);
        }
        //[HttpPost]
        //public async Task<IActionResult> Create()
        //{

        //}
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var userId = id;
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier);
                await _adminService.DeleteDataUser(adminId.Value, userId);
                TempData.Put("Alert",new ErrorMessageViewModel { AlertType = AlertType.info.ToString(), Message = "Delete success" });
                return RedirectToAction("DashBoard");
            }catch 
            {
                TempData.Put("Alert", new ErrorMessageViewModel { AlertType = AlertType.info.ToString(), Message = "Can not delete user." });

                return RedirectToAction("DashBoard");

            }

        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditRequest editRequest)
        {
        if (!ModelState.IsValid) {
            TempData["Alert"] = new ErrorMessageViewModel(){ AlertType=AlertType.info.ToString(),Message="Invalid inputs." };
                return View(editRequest);
            }
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier);
                await _adminService.UpdateUser(userId.Value, editRequest);
                return RedirectToAction("DashBoard");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Alert"] = new ErrorMessageViewModel() { AlertType = AlertType.warning.ToString(), Message = "Internal error." };
                return View(editRequest);
            }

        }


    }
}
