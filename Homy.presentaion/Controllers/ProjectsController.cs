using Homy.Application.Dtos;
using Homy.Domin.Contract_Service;
using Homy.Infurastructure.Unitofworks;
using Microsoft.AspNetCore.Mvc;

namespace Homy.presentaion.Controllers
{
     public class ProjectsController : Controller
    {
        private readonly IProject_Service _projectService;
        private readonly ICity_Service _cityService;
        private readonly IDistrict_Service _districtService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            IProject_Service projectService,
            ICity_Service cityService,
            IDistrict_Service districtService,
            ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _cityService = cityService;
            _districtService = districtService;
            _logger = logger;
        }

        /// <summary>
        /// GET: /Projects
        /// صفحة عرض كل المشاريع
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = null, long? cityId = null)
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

        /// <summary>
        /// GET: /Projects/Details/5
        /// صفحة تفاصيل مشروع معين
        /// </summary>
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

        /// <summary>
        /// GET: /Projects/Create
        /// صفحة إضافة مشروع جديد
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                // جلب المدن والأحياء للـ Dropdown
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

        /// <summary>
        /// POST: /Projects/Create
        /// إضافة مشروع جديد
        /// </summary>
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

                // جلب User Id من الـ Claims
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

        /// <summary>
        /// GET: /Projects/Edit/5
        /// صفحة تعديل مشروع
        /// </summary>
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
                    LogoUrl = project.LogoUrl,
                    CoverImageUrl = project.CoverImageUrl,
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

        /// <summary>
        /// POST: /Projects/Edit/5
        /// تعديل بيانات مشروع
        /// </summary>
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

        /// <summary>
        /// POST: /Projects/Delete/5
        /// حذف مشروع (Soft Delete)
        /// </summary>
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

        /// <summary>
        /// POST: /Projects/ToggleStatus/5
        /// تفعيل/إلغاء تفعيل مشروع
        /// </summary>
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

        /// <summary>
        /// GET: /Projects/ActiveProjects
        /// صفحة المشاريع النشطة فقط
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ActiveProjects()
        {
            try
            {
                var projects = await _projectService.GetActiveProjectsAsync();
                var stats = await _projectService.GetProjectStatsAsync();
                
                ViewBag.Stats = stats;
                ViewBag.FilterType = "Active";

                return View("Index", projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب المشاريع النشطة");
                TempData["ErrorMessage"] = "حدث خطأ أثناء جلب البيانات";
                return RedirectToAction(nameof(Index));
            }
        }

        #region Helper Methods

        /// <summary>
        /// جلب User ID من الـ Claims
        /// </summary>
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        /// <summary>
        /// تحميل المدن والأحياء في ViewBag
        /// </summary>
        private async Task LoadCitiesAndDistricts()
        {
            // TODO: تنفيذ GetAllCitiesAsync و GetAllDistrictsAsync في Services
            // مؤقتاً نستخدم Repository مباشرة
            ViewBag.Cities = await _cityService.GetAllCitiesAsync();
            ViewBag.Districts = await _districtService.GetAllDistrictsAsync();
        }

        #endregion
    }
}
