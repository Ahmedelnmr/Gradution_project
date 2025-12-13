using Homy.Domin.Contract_Repo;
using Homy.Domin.models;

namespace Homy.Domin.Contract_Repo
{
    public interface IProject_Repo : IGenric_Repo<Project>
    {
        /// <summary>
        /// جلب كل المشاريع مع بيانات المدن والأحياء
        /// </summary>
        Task<IEnumerable<Project>> GetAllWithDetailsAsync();

        /// <summary>
        /// جلب مشروع واحد مع كل التفاصيل والعلاقات
        /// </summary>
        Task<Project> GetByIdWithDetailsAsync(long id);

        /// <summary>
        /// جلب المشاريع حسب المدينة
        /// </summary>
        Task<IEnumerable<Project>> GetByCityIdAsync(long cityId);

        /// <summary>
        /// جلب المشاريع حسب الحي
        /// </summary>
        Task<IEnumerable<Project>> GetByDistrictIdAsync(long districtId);

        /// <summary>
        /// جلب المشاريع النشطة فقط
        /// </summary>
        Task<IEnumerable<Project>> GetActiveProjectsAsync();

        /// <summary>
        /// البحث في المشاريع حسب الاسم
        /// </summary>
        Task<IEnumerable<Project>> SearchByNameAsync(string searchTerm);

        /// <summary>
        /// جلب عدد الوحدات في مشروع معين
        /// </summary>
        Task<int> GetPropertiesCountAsync(long projectId);

        /// <summary>
        /// جلب أقل سعر للوحدات في مشروع
        /// </summary>
        Task<decimal?> GetMinPriceAsync(long projectId);

        /// <summary>
        /// جلب المشاريع مع عدد الوحدات
        /// </summary>
        Task<IEnumerable<(Project Project, int PropertiesCount)>> GetProjectsWithCountAsync();
    }
}