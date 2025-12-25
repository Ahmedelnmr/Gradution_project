using Homy.Application.Dtos.Admin;
using Homy.Domin.Contract_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Homy.presentaion.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class AdminPropertyTypesController : Controller
    {
        private readonly IPropertyType_AdminService _propertyTypeService;

        public AdminPropertyTypesController(IPropertyType_AdminService propertyTypeService)
        {
            _propertyTypeService = propertyTypeService;
        }

        // GET: /AdminPropertyTypes
        public async Task<IActionResult> Index()
        {
            var types = await _propertyTypeService.GetAllAsync();
            return View(types);
        }

        // GET: /AdminPropertyTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /AdminPropertyTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePropertyTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _propertyTypeService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminPropertyTypes/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var type = await _propertyTypeService.GetByIdAsync(id);
            if (type == null) return NotFound();

            var updateDto = new UpdatePropertyTypeDto
            {
                Id = type.Id,
                Name = type.Name,
                CurrentIconUrl = type.IconUrl  // For preview
            };

            return View(updateDto);
        }

        // POST: /AdminPropertyTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePropertyTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            // Handle image upload
            if (dto.IconImage != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "property-types");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{dto.IconImage.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.IconImage.CopyToAsync(stream);
                }

                dto.CurrentIconUrl = $"/uploads/property-types/{uniqueFileName}";
            }

            var result = await _propertyTypeService.UpdateAsync(dto);
            if (!result) return NotFound();

            TempData["Success"] = "تم تحديث نوع العقار بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminPropertyTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var (success, message) = await _propertyTypeService.DeleteAsync(id);
            
            if (!success)
            {
                TempData["ErrorMessage"] = message;
            }
            else
            {
                TempData["SuccessMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
