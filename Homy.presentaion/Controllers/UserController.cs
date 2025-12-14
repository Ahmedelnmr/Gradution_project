using Homy.Application.Contract_Service;
using Homy.Application.Dtos.UserDtos;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Homy.Web.Controllers
{
    
    //[Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUser_Service _userService;

        public UsersController(IUser_Service userService)
        {
            _userService = userService;
        }


        public async Task<IActionResult> Index(UserFilterDto filter)
        {
            var result = await _userService.GetAllUsersAsync(filter);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                ViewBag.TotalCount = 0;
                ViewBag.CurrentFilter = filter;
                return View(new List<UserDto>());
            }

            ViewBag.TotalCount = result.TotalCount;
            ViewBag.CurrentFilter = filter;

            return View(result.Data);
        }


        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

       
        public async Task<IActionResult> UnverifiedAgents()
        {
            var result = await _userService.GetUnverifiedAgentsAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(new List<UserDto>());
            }

            TempData["Info"] = result.Message; 
            return View(result.Data);
        }

        
        public async Task<IActionResult> VerifyAgent(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(UnverifiedAgents));
            }

            
            if (result.Data.Role != UserRole.Agent)
            {
                TempData["Error"] = "المستخدم ليس سمسار";
                return RedirectToAction(nameof(Index));
            }

            var verificationRequest = new VerificationRequestDto
            {
                UserId = id
            };

            ViewBag.UserInfo = result.Data; 
            return View(verificationRequest);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyAgent(VerificationRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "يرجى ملء البيانات بشكل صحيح";
                return RedirectToAction(nameof(VerifyAgent), new { id = request.UserId });
            }

            // لو رافض، لازم يكتب سبب
            if (!request.IsApproved && string.IsNullOrWhiteSpace(request.Reason))
            {
                TempData["Error"] = "يجب كتابة سبب الرفض";
                return RedirectToAction(nameof(VerifyAgent), new { id = request.UserId });
            }

            var result = await _userService.VerifyAgentAsync(request);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(UnverifiedAgents));
            }

            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(VerifyAgent), new { id = request.UserId });
        }

        
        [HttpPost]
        public async Task<IActionResult> ToggleActive(Guid userId, bool isActive)
        {
            var request = new UpdateUserStatusDto
            {
                UserId = userId,
                IsActive = isActive
            };

            var result = await _userService.UpdateUserActiveStatusAsync(request);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Delete), new { id });
        }

        
        public async Task<IActionResult> Statistics()
        {
            var result = await _userService.GetUserStatisticsAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(new UserStatisticsDto());
            }

            return View(result.Data);
        }

      
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new { success = false, message = "يجب إدخال كلمة البحث" });
            }

            var result = await _userService.SearchUsersAsync(term);

            if (result.Success)
            {
                return Json(new { success = true, data = result.Data });
            }

            return Json(new { success = false, message = result.Message });
        }

       
        [HttpPost]
        public async Task<IActionResult> FilterUsers([FromBody] UserFilterDto filter)
        {
            var result = await _userService.GetAllUsersAsync(filter);

            if (result.Success)
            {
                return Json(new { success = true, data = result.Data });
            }

            return Json(new { success = false, message = result.Message });
        }
    }
}