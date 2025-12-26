using Homy.Application.Dtos;
using Homy.Application.Dtos.Admin;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Microsoft.AspNetCore.Hosting; // For file uploads
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Homy.presentaion.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class AdminPropertiesController : Controller
    {
        private readonly IAdminProperty_Service _propertyService;
        private readonly ICity_Service _cityService;
        private readonly IDistrict_Service _districtService;
        private readonly IPropertyType_AdminService _propertyTypeService;
        private readonly IProject_Service _projectService;
        
        // Removed logger to keep it simple as per existing style, or could add it.
        // Assuming simple injection for now.

        public AdminPropertiesController(
            IAdminProperty_Service propertyService,
            ICity_Service cityService,
            IDistrict_Service districtService,
            IPropertyType_AdminService propertyTypeService,
            IProject_Service projectService)
        {
            _propertyService = propertyService;
            _cityService = cityService;
            _districtService = districtService;
            _propertyTypeService = propertyTypeService;
            _projectService = projectService;
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

        // GET: /AdminProperties/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookups();
            return View(new CreatePropertyDto());
        }

        // POST: /AdminProperties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePropertyDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookups();
                return View(dto);
            }

            try
            {
                var adminId = GetCurrentUserId();
                if (adminId == Guid.Empty) return RedirectToAction("Login", "Account");

                // Handle Image Uploads
                if (dto.Images != null && dto.Images.Any())
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "properties");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    foreach (var image in dto.Images)
                    {
                        var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        dto.ImageUrls.Add($"/uploads/properties/{fileName}");
                    }
                }

                await _propertyService.CreatePropertyAsync(dto, adminId);
                TempData["Success"] = "تم إضافة العقار بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "حدث خطأ أثناء الإضافة: " + ex.Message);
                await LoadLookups();
                return View(dto);
            }
        }

        // GET: /AdminProperties/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var dto = await _propertyService.GetPropertyForEditAsync(id);
            if (dto == null)
            {
                TempData["Error"] = "العقار غير موجود";
                return RedirectToAction(nameof(Index));
            }

            await LoadLookups();
            return View(dto);
        }

        // POST: /AdminProperties/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePropertyDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Reload existing images for display if validation fails
                var existingDto = await _propertyService.GetPropertyForEditAsync(dto.Id);
                if (existingDto != null)
                {
                    dto.CurrentImages = existingDto.CurrentImages;
                }
                
                await LoadLookups();
                return View(dto);
            }

            try
            {
                var adminId = GetCurrentUserId();
                if (adminId == Guid.Empty) return RedirectToAction("Login", "Account");

                // Handle New Images
                if (dto.NewImages != null && dto.NewImages.Any())
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "properties");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    foreach (var image in dto.NewImages)
                    {
                        var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        dto.NewImageUrls.Add($"/uploads/properties/{fileName}");
                    }
                }

                await _propertyService.UpdatePropertyAsync(dto, adminId);
                TempData["Success"] = "تم تحديث العقار بنجاح";
                return RedirectToAction(nameof(Details), new { id = dto.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "حدث خطأ أثناء التحديث: " + ex.Message);
                
                // Reload images on error
                var existingDto = await _propertyService.GetPropertyForEditAsync(dto.Id);
                if (existingDto != null)
                {
                    dto.CurrentImages = existingDto.CurrentImages;
                }

                await LoadLookups();
                return View(dto);
            }
        }

        // POST: /AdminProperties/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _propertyService.DeletePropertyAsync(id);
            if (success)
                TempData["Success"] = "تم حذف العقار بنجاح";
            else
                TempData["Error"] = "العقار غير موجود";

            return RedirectToAction(nameof(Index));
        }


        // POST: /AdminProperties/Review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(CreatePropertyReviewDto dto)
        {
            var adminId = GetCurrentUserId();
            if (adminId == Guid.Empty) return RedirectToAction("Login", "Account");

            var success = await _propertyService.ReviewPropertyAsync(adminId, dto);
            
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

        private async Task LoadLookups()
        {
            ViewBag.Cities = await _cityService.GetAllCitiesAsync();
            ViewBag.Districts = await _districtService.GetAllDistrictsAsync();
            ViewBag.PropertyTypes = await _propertyTypeService.GetAllAsync();
            ViewBag.Projects = await _projectService.GetProjectsListAsync();
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userId) ? Guid.Empty : Guid.Parse(userId);
        }
    }
}
