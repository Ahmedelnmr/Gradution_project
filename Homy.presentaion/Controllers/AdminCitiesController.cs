using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Homy.presentaion.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class AdminCitiesController : Controller
    {
        private readonly ICity_Service _cityService;

        public AdminCitiesController(ICity_Service cityService)
        {
            _cityService = cityService;
        }

        // GET: /AdminCities
        public async Task<IActionResult> Index()
        {
            var cities = await _cityService.GetAllCitiesWithDistrictsAsync();
            return View(cities);
        }

        // GET: /AdminCities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /AdminCities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string nameEn)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "اسم المدينة مطلوب");
                return View();
            }

            // Using User identifier (simplified for now, ideally retrieve proper ID)
            Guid? adminId = null;
            var adminIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(adminIdStr, out var parsedId)) adminId = parsedId;

            await _cityService.CreateCityAsync(name, nameEn, adminId);
            TempData["Success"] = "تم إضافة المدينة بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminCities/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var city = await _cityService.GetCityByIdAsync(id);
            if (city == null) return NotFound();
            return View(city);
        }

        // POST: /AdminCities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, string nameEn)
        {
             if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "اسم المدينة مطلوب");
                var city = await _cityService.GetCityByIdAsync(id);
                return View(city);
            }

            Guid? adminId = null;
            var adminIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(adminIdStr, out var parsedId)) adminId = parsedId;

            await _cityService.UpdateCityAsync(id, name, nameEn, adminId);
            TempData["Success"] = "تم تحديث المدينة بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminCities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _cityService.DeleteCityAsync(id);
            if (success)
                TempData["Success"] = "تم حذف المدينة بنجاح";
            else
                TempData["Error"] = "فشل حذف المدينة";

            return RedirectToAction(nameof(Index));
        }
    }
}
