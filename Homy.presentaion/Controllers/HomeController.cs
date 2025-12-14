using Homy.presentaion.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Homy.Domin.Contract_Service;

namespace Homy.presentaion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboard_Service _dashboardService;

        public HomeController(ILogger<HomeController> logger, IDashboard_Service dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardDto = await _dashboardService.GetDashboardDataAsync();
            return View(dashboardDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
