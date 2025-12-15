using Homy.Application.Dtos.UserDtos.UserSubDTOs;
using Homy.Domin.Contract_Service;
using Microsoft.AspNetCore.Mvc;

namespace Homy.presentaion.Controllers
{
    public class UserSubscriptionController : Controller
    {
        private readonly IUserSubscription_Service _userSubscriptionService;

        public UserSubscriptionController(IUserSubscription_Service userSubscriptionService)
        {
            _userSubscriptionService = userSubscriptionService;
        }

        public async Task<IActionResult> GetAll()
        {
            var subs = await _userSubscriptionService.GetAllSubscriptionsAsync();
            return View(subs);
        }

        public async Task<IActionResult> Details(Guid userId)
        {
            var sub = await _userSubscriptionService.GetUserSubscriptionAsync(userId);
            if (sub == null)
                return NotFound();

            return View(sub);
        }

        public async Task<IActionResult> ByPackage(long packageId)
        {
            var subs = await _userSubscriptionService.GetSubscriptionsByPackageAsync(packageId);
            return View(subs);
        }


        public async Task<IActionResult> Expired()
        {
            var subs = await _userSubscriptionService.GetExpiredSubscriptionsAsync();
            return View(subs);
        }

        public async Task<IActionResult> ExpiringSoon()
        {
            var subs = await _userSubscriptionService.GetExpiringIn7DaysAsync();
            return View(subs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserSubCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _userSubscriptionService.CreateSubscriptionAsync(dto);
            return RedirectToAction(nameof(ExpiringSoon));
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid userId)
        {
            var sub = await _userSubscriptionService.GetUserSubscriptionAsync(userId);
            if (sub == null)
                return NotFound();

            var updateDto = new Homy.Application.Dtos.UserDtos.UserSubDTOs.UserSubUpdateDTO
            {
                EndDate = sub.EndDate,
                IsActive = sub.IsActive
            };

            ViewData["UserId"] = userId;
            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid userId, UserSubUpdateDTO dto)
        {
            var result = await _userSubscriptionService.UpdateSubscriptionAsync(userId, dto);
            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Details), new { userId });
        }
    }
}
