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
        public async Task<IActionResult> Index(PropertyFilterDto filter)
        {
            var result = await _propertyService.GetPropertiesAsync(filter);
            var counts = await _propertyService.GetStatusCountsAsync();
            ViewBag.StatusCounts = counts;
            return View(result);
        }

        // GET: /AdminProperties/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var property = await _propertyService.GetPropertyDetailsAsync(id);
            if (property == null) return NotFound();
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
                return RedirectToAction("Login", "Account"); // Or handle as unauthorized
            }

            await _propertyService.ReviewPropertyAsync(Guid.Parse(adminId), dto);
            
            // Return to details with clear cache or updated status
            return RedirectToAction(nameof(Details), new { id = dto.PropertyId });
        }
    }
}
