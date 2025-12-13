using Homy.Application.Dtos;

namespace Homy.Domin.Contract_Service
{
    public interface IProject_Service
    {
        /// <summary>
        /// جلب كل المشاريع مع التفاصيل الكاملة
        /// </summary>
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();

        /// <summary>
        /// جلب قائمة مبسطة للمشاريع
        /// </summary>
        Task<IEnumerable<ProjectListDto>> GetProjectsListAsync();

        /// <summary>
        /// جلب مشروع واحد بالتفاصيل
        /// </summary>
        Task<ProjectDto> GetProjectByIdAsync(long id);

        /// <summary>
        /// إنشاء مشروع جديد
        /// </summary>
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto createDto, Guid? createdById);

        /// <summary>
        /// تحديث بيانات مشروع
        /// </summary>
        Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto updateDto, Guid? updatedById);

        /// <summary>
        /// حذف مشروع (Soft Delete)
        /// </summary>
        Task<bool> DeleteProjectAsync(long id);

        /// <summary>
        /// تفعيل/إلغاء تفعيل مشروع
        /// </summary>
        Task<bool> ToggleProjectStatusAsync(long id);

        /// <summary>
        /// البحث في المشاريع
        /// </summary>
        Task<IEnumerable<ProjectListDto>> SearchProjectsAsync(string searchTerm);

        /// <summary>
        /// جلب المشاريع حسب المدينة
        /// </summary>
        Task<IEnumerable<ProjectListDto>> GetProjectsByCityAsync(long cityId);

        /// <summary>
        /// جلب المشاريع النشطة فقط
        /// </summary>
        Task<IEnumerable<ProjectListDto>> GetActiveProjectsAsync();

        /// <summary>
        /// جلب إحصائيات المشاريع
        /// </summary>
        Task<ProjectStatsDto> GetProjectStatsAsync();
    }   
}