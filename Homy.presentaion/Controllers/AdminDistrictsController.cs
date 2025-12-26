using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Homy.presentaion.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class AdminDistrictsController : Controller
    {
        private readonly IDistrict_Service _districtService;
        private readonly ICity_Service _cityService;

        public AdminDistrictsController(IDistrict_Service districtService, ICity_Service cityService)
        {
            _districtService = districtService;
            _cityService = cityService;
        }

        // GET: /AdminDistricts
        public async Task<IActionResult> Index(long? cityId)
        {
            // Load cities for filter
            ViewBag.Cities = await _cityService.GetAllCitiesAsync();
            ViewBag.CurrentCityId = cityId;

            IEnumerable<District> districts;
            if (cityId.HasValue)
            {
                districts = await _districtService.GetDistrictsByCityIdAsync(cityId.Value);
            }
            else
            {
                districts = await _districtService.GetAllDistrictsAsync();
            }

            return View(districts);
        }

        // GET: /AdminDistricts/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Cities = await _cityService.GetAllCitiesAsync();
            return View();
        }

        // POST: /AdminDistricts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string nameEn, long cityId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "اسم الحي مطلوب");
            }

            if (cityId <= 0)
            {
                ModelState.AddModelError("CityId", "يجب اختيار المدينة");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Cities = await _cityService.GetAllCitiesAsync();
                return View();
            }

            await _districtService.CreateDistrictAsync(name, nameEn, cityId);
            TempData["Success"] = "تم إضافة الحي بنجاح";
            return RedirectToAction(nameof(Index), new { cityId });
        }

        // GET: /AdminDistricts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var district = await _districtService.GetDistrictByIdAsync(id);
            if (district == null) return NotFound();

            ViewBag.Cities = await _cityService.GetAllCitiesAsync();
            return View(district);
        }

        // POST: /AdminDistricts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, string nameEn, long cityId)
        {
            if (string.IsNullOrWhiteSpace(name))
                ModelState.AddModelError("Name", "اسم الحي مطلوب");
            
            if (cityId <= 0)
                ModelState.AddModelError("CityId", "يجب اختيار المدينة");

            if (!ModelState.IsValid)
            {
                var district = await _districtService.GetDistrictByIdAsync(id);
                ViewBag.Cities = await _cityService.GetAllCitiesAsync();
                return View(district);
            }

            await _districtService.UpdateDistrictAsync(id, name, nameEn, cityId);
            TempData["Success"] = "تم تحديث الحي بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminDistricts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _districtService.DeleteDistrictAsync(id);
            if (success)
                TempData["Success"] = "تم حذف الحي بنجاح";
            else
                TempData["Error"] = "فشل حذف الحي";

            return RedirectToAction(nameof(Index));
        }
    }
}
