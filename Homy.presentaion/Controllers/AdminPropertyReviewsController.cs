using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Homy.presentaion.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class AdminPropertyReviewsController : Controller
    {
        private readonly IPropertyReview_Service _reviewService;

        public AdminPropertyReviewsController(IPropertyReview_Service reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                return View(reviews);
            }
            catch (Exception ex)
            {
                // Log the exception (you might want to inject ILogger)
                TempData["Error"] = "حدث خطأ أثناء تحميل السجلات: " + ex.Message;
                return View(new List<Homy.Domin.models.PropertyReview>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _reviewService.DeleteReviewAsync(id);
            if (success)
            {
                TempData["Success"] = "تم حذف السجل بنجاح";
            }
            else
            {
                TempData["Error"] = "فشل في الحذف";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
