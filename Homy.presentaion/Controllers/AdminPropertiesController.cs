using Homy.Application.Dtos.Admin;
using Homy.Domin.Contract_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Homy.presentaion.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class AdminPropertiesController : Controller
    {
        private readonly IAdminProperty_Service _propertyService;

        public AdminPropertiesController(IAdminProperty_Service propertyService)
        {
            _propertyService = propertyService;
        }

        // GET: /AdminProperties
        public async Task<IActionResult> Index(PropertyFilterDto? filter)
        {
            // Initialize filter with defaults if null
            filter ??= new PropertyFilterDto();
            
            // Set default page size if not specified
            if (filter.PageSize == 0)
                filter.PageSize = 12;
            
            if (filter.Page == 0)
                filter.Page = 1;

            var result = await _propertyService.GetPropertiesAsync(filter);
            var counts = await _propertyService.GetStatusCountsAsync();
            
            ViewBag.StatusCounts = counts;
            ViewBag.CurrentFilter = filter;
            
            return View(result);
        }

        // GET: /AdminProperties/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var property = await _propertyService.GetPropertyDetailsAsync(id);
            
            if (property == null)
            {
                TempData["Error"] = "الإعلان غير موجود";
                return RedirectToAction(nameof(Index));
            }
            
            return View(property);
        }

        // POST: /AdminProperties/Review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(CreatePropertyReviewDto dto)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
            {
                TempData["Error"] = "يجب تسجيل الدخول أولاً";
                return RedirectToAction("Login", "Account");
            }

            var success = await _propertyService.ReviewPropertyAsync(Guid.Parse(adminId), dto);
            
            if (success)
            {
                var actionText = dto.Action switch
                {
                    Homy.Domin.models.ReviewAction.Approved => "تم قبول الإعلان بنجاح",
                    Homy.Domin.models.ReviewAction.Rejected => "تم رفض الإعلان",
                    Homy.Domin.models.ReviewAction.ChangesRequested => "تم طلب التعديلات",
                    _ => "تم تحديث حالة الإعلان"
                };
                TempData["Success"] = actionText;
            }
            else
            {
                TempData["Error"] = "حدث خطأ أثناء مراجعة الإعلان";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
