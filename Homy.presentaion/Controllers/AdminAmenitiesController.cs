using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Homy.presentaion.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class AdminAmenitiesController : Controller
    {
        private readonly IAmenity_Service _amenityService;

        public AdminAmenitiesController(IAmenity_Service amenityService)
        {
            _amenityService = amenityService;
        }

        // GET: /AdminAmenities
        public async Task<IActionResult> Index()
        {
            var amenities = await _amenityService.GetAllAmenitiesAsync();
            return View(amenities);
        }

        // GET: /AdminAmenities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /AdminAmenities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string nameEn, IFormFile? iconFile)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "اسم الميزة مطلوب");
                return View();
            }

            string? iconUrl = null;
            if (iconFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "amenities");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{iconFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await iconFile.CopyToAsync(stream);
                }
                iconUrl = $"/uploads/amenities/{uniqueFileName}";
            }

            await _amenityService.CreateAmenityAsync(name, nameEn, iconUrl);
            TempData["Success"] = "تم إضافة الميزة بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminAmenities/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var amenity = await _amenityService.GetAmenityByIdAsync(id);
            if (amenity == null) return NotFound();
            return View(amenity);
        }

        // POST: /AdminAmenities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, string nameEn, IFormFile? iconFile)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "اسم الميزة مطلوب");
                var amenity = await _amenityService.GetAmenityByIdAsync(id);
                return View(amenity);
            }

            string? iconUrl = null;
            if (iconFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "amenities");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{iconFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await iconFile.CopyToAsync(stream);
                }
                iconUrl = $"/uploads/amenities/{uniqueFileName}";
            }

            await _amenityService.UpdateAmenityAsync(id, name, nameEn, iconUrl);
            TempData["Success"] = "تم تحديث الميزة بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminAmenities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _amenityService.DeleteAmenityAsync(id);
            if (success)
                TempData["Success"] = "تم حذف الميزة بنجاح";
            else
                TempData["Error"] = "فشل حذف الميزة";

            return RedirectToAction(nameof(Index));
        }
    }
}
