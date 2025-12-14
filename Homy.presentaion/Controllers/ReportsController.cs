using Homy.Application.Contract_Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Homy.presentaion.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReports_Service _reportsService;

        public ReportsController(IReports_Service reportsService)
        {
            _reportsService = reportsService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var reports = await _reportsService.GetFullReportsAsync();
                return View(reports);
            }
            catch (System.Exception ex)
            {
                // Log the exception
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل التقارير";
                return View();
            }
        }

        // API للحصول على البيانات بصيغة JSON
        [HttpGet]
        public async Task<IActionResult> GetReportsData()
        {
            try
            {
                var reports = await _reportsService.GetFullReportsAsync();
                return Json(new { success = true, data = reports });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API للحصول على عدد المستخدمين فقط
        [HttpGet]
        public async Task<IActionResult> GetUsersCount()
        {
            var count = await _reportsService.GetUsersCountAsync();
            return Json(new { success = true, count });
        }

        // API للحصول على عدد الإعلانات فقط
        [HttpGet]
        public async Task<IActionResult> GetAdsCount()
        {
            var count = await _reportsService.GetAdsCountAsync();
            return Json(new { success = true, count });
        }

        // API للحصول على الإيرادات الشهرية لسنة معينة
        [HttpGet]
        public async Task<IActionResult> GetMonthlyRevenue(int year)
        {
            // If year is 0 or invalid, use current year
            if (year <= 0) year = System.DateTime.Now.Year;
            
            var data = await _reportsService.GetMonthlyRevenuesByYearAsync(year);
            // Calculate total revenue for this year from the monthly data
            var totalRevenue = data.Sum(m => m.Revenue);
            
            return Json(new { success = true, data, totalRevenue });
        }
    }
}