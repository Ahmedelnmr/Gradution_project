using Homy.Application.Dtos;
using Homy.Domin.Contract_Service;
using Homy.Infurastructure.Unitofworks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.presentaion.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IProject_Service _projectService;
        private readonly ICity_Service _cityService;
        private readonly IDistrict_Service _districtService;
        private readonly ILogger<ProjectsController> _logger;
        private readonly IUnitofwork _unitofwork;

        public ProjectsController(
            IProject_Service projectService,
            ICity_Service cityService,
            IDistrict_Service districtService,
            ILogger<ProjectsController> logger,
            IUnitofwork unitofwork)
        {
            _projectService = projectService;
            _cityService = cityService;
            _districtService = districtService;
            _logger = logger;
            _unitofwork = unitofwork;
        }

        /// <summary>
        /// GET: /Projects
        /// صفحة عرض كل المشاريع مع الفلترة
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = null, long? cityId = null, string filter = null)
        {
            try
            {
                IEnumerable<ProjectListDto> projects;

                // تطبيق الفلاتر
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    projects = await _projectService.SearchProjectsAsync(searchTerm);
                    ViewBag.SearchTerm = searchTerm;
                }
                else if (cityId.HasValue)
                {
                    projects = await _projectService.GetProjectsByCityAsync(cityId.Value);
                    ViewBag.CityId = cityId.Value;
                }
                else if (!string.IsNullOrWhiteSpace(filter))
                {
                    projects = filter.ToLower() switch
                    {
                        "active" => await _projectService.GetActiveProjectsAsync(),
                        "residential" => await _projectService.GetProjectsByComputedTypeAsync(ProjectTypeEnum.ResidentialCompound),
                        "commercial" => await _projectService.GetProjectsByComputedTypeAsync(ProjectTypeEnum.CommercialProject),
                        "construction" => await _projectService.GetProjectsByComputedTypeAsync(ProjectTypeEnum.UnderConstruction),
                        _ => await _projectService.GetProjectsListAsync()
                    };
                    ViewBag.CurrentFilter = filter;
                }
                else
                {
                    projects = await _projectService.GetProjectsListAsync();
                }

                // جلب الإحصائيات
                var stats = await _projectService.GetProjectStatsAsync();
                ViewBag.Stats = stats;

                // جلب المدن للفلتر
                var cities = await _cityService.GetAllCitiesAsync();
                ViewBag.Cities = cities;

                return View(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب قائمة المشاريع");
                TempData["ErrorMessage"] = "حدث خطأ أثناء جلب البيانات";
                return View(new List<ProjectListDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);

                if (project == null)
                {
                    TempData["ErrorMessage"] = "المشروع غير موجود";
                    return RedirectToAction(nameof(Index));
                }

                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطأ في جلب تفاصيل المشروع {id}");
                TempData["ErrorMessage"] = "حدث خطأ أثناء جلب التفاصيل";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                await LoadCitiesAndDistricts();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحميل صفحة إضافة مشروع");
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل الصفحة";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadCitiesAndDistricts();
                    return View(createDto);
                }

                // Handle image uploads
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "projects");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Handle Logo Upload
                if (createDto.LogoImage != null)
                {
                    var logoFileName = $"logo_{Guid.NewGuid()}_{createDto.LogoImage.FileName}";
                    var logoPath = Path.Combine(uploadsFolder, logoFileName);
                    
                    using (var stream = new FileStream(logoPath, FileMode.Create))
                    {
                        await createDto.LogoImage.CopyToAsync(stream);
                    }
                    
                    createDto.LogoUrl = $"/uploads/projects/{logoFileName}";
                }

                // Handle Cover Upload
                if (createDto.CoverImage != null)
                {
                    var coverFileName = $"cover_{Guid.NewGuid()}_{createDto.CoverImage.FileName}";
                    var coverPath = Path.Combine(uploadsFolder, coverFileName);
                    
                    using (var stream = new FileStream(coverPath, FileMode.Create))
                    {
                        await createDto.CoverImage.CopyToAsync(stream);
                    }
                    
                    createDto.CoverImageUrl = $"/uploads/projects/{coverFileName}";
                }

                var userId = GetCurrentUserId();
                var createdProject = await _projectService.CreateProjectAsync(createDto, userId);

                TempData["SuccessMessage"] = $"تم إضافة المشروع '{createdProject.Name}' بنجاح";
                return RedirectToAction(nameof(Details), new { id = createdProject.Id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadCitiesAndDistricts();
                return View(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إضافة مشروع جديد");
                ModelState.AddModelError("", "حدث خطأ أثناء إضافة المشروع");
                await LoadCitiesAndDistricts();
                return View(createDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);

                if (project == null)
                {
                    TempData["ErrorMessage"] = "المشروع غير موجود";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdateProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    CurrentLogoUrl = project.LogoUrl,        // ✅ For preview
                    CurrentCoverUrl = project.CoverImageUrl, // ✅ For preview
                    CityId = project.CityId,
                    DistrictId = project.DistrictId,
                    LocationDescription = project.LocationDescription,
                    IsActive = project.IsActive
                };

                await LoadCitiesAndDistricts();
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطأ في تحميل صفحة تعديل المشروع {id}");
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل الصفحة";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProjectDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadCitiesAndDistricts();
                    return View(updateDto);
                }

                // ✅ Handle image uploads
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "projects");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // ✅ Handle Logo Upload
                if (updateDto.LogoImage != null)
                {
                    var logoFileName = $"logo_{Guid.NewGuid()}_{updateDto.LogoImage.FileName}";
                    var logoPath = Path.Combine(uploadsFolder, logoFileName);
                    
                    using (var stream = new FileStream(logoPath, FileMode.Create))
                    {
                        await updateDto.LogoImage.CopyToAsync(stream);
                    }
                    
                    updateDto.LogoUrl = $"/uploads/projects/{logoFileName}";
                }

                // ✅ Handle Cover Upload
                if (updateDto.CoverImage != null)
                {
                    var coverFileName = $"cover_{Guid.NewGuid()}_{updateDto.CoverImage.FileName}";
                    var coverPath = Path.Combine(uploadsFolder, coverFileName);
                    
                    using (var stream = new FileStream(coverPath, FileMode.Create))
                    {
                        await updateDto.CoverImage.CopyToAsync(stream);
                    }
                    
                    updateDto.CoverImageUrl = $"/uploads/projects/{coverFileName}";
                }

                var userId = GetCurrentUserId();
                var updatedProject = await _projectService.UpdateProjectAsync(updateDto, userId);

                TempData["SuccessMessage"] = $"تم تحديث المشروع '{updatedProject.Name}' بنجاح";
                return RedirectToAction(nameof(Details), new { id = updatedProject.Id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadCitiesAndDistricts();
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطأ في تعديل المشروع {updateDto.Id}");
                ModelState.AddModelError("", "حدث خطأ أثناء تحديث المشروع");
                await LoadCitiesAndDistricts();
                return View(updateDto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var success = await _projectService.DeleteProjectAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "تم حذف المشروع بنجاح";
                }
                else
                {
                    TempData["ErrorMessage"] = "المشروع غير موجود";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطأ في حذف المشروع {id}");
                TempData["ErrorMessage"] = "حدث خطأ أثناء حذف المشروع";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            try
            {
                var success = await _projectService.ToggleProjectStatusAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "تم تغيير حالة المشروع بنجاح";
                }
                else
                {
                    TempData["ErrorMessage"] = "المشروع غير موجود";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطأ في تغيير حالة المشروع {id}");
                TempData["ErrorMessage"] = "حدث خطأ أثناء تغيير الحالة";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Projects/{id}/Units
        [HttpGet]
        public async Task<IActionResult> Units(long id)
        {
            try
            {
                // Get project details for display
                var project = await _projectService.GetProjectByIdAsync(id);
                if (project == null)
                {
                    TempData["ErrorMessage"] = "المشروع غير موجود";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ProjectId = id;
                ViewBag.ProjectName = project.Name;

                // Get all properties for this project
                var properties = await _unitofwork.PropertyRepo.GetAll()
                    .Where(p => p.ProjectId == id && !p.IsDeleted)
                    .Include(p => p.PropertyType)
                    .Include(p => p.City)
                    .Include(p => p.District)
                    .Include(p => p.User)
                    .Include(p => p.Images)
                    .ToListAsync();

                return View(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطأ في جلب وحدات المشروع {id}");
                TempData["ErrorMessage"] = "حدث خطأ أثناء جلب الوحدات";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #region Helper Methods

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        private async Task LoadCitiesAndDistricts()
        {
            ViewBag.Cities = await _cityService.GetAllCitiesAsync();
            ViewBag.Districts = await _districtService.GetAllDistrictsAsync();
        }

        #endregion
    }
}