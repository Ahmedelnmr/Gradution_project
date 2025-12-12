using Homy.presentaion.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Homy.Domin.models;
using Homy.Domin.Contract_Repo;
using Microsoft.AspNetCore.Identity;
using Homy.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Homy.presentaion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IProperty_Repo _propertyRepo;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, IProperty_Repo propertyRepo)
        {
            _logger = logger;
            _userManager = userManager;
            _propertyRepo = propertyRepo;
        }

        public async Task<IActionResult> Index()
        {
            var usersCount = await _userManager.Users.CountAsync();
            var adsCount = await _propertyRepo.CountAsync();

            var dashboardDto = new DashboardDto
            {
                UsersCount = usersCount,
                AdsCount = adsCount
            };

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
