using Homy.Domin.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homy.Domin.models;

namespace Homy.Domin.Contract_Repo
{
    public interface ICity_Repo : IGenric_Repo<City>
    {
        /// <summary>
        /// جلب كل المدن مع الأحياء التابعة لها
        /// </summary>
        Task<IEnumerable<City>> GetAllWithDistrictsAsync();

        /// <summary>
        /// جلب مدينة واحدة مع كل التفاصيل (الأحياء والعقارات والمشاريع)
        /// </summary>
        Task<City> GetByIdWithDetailsAsync(long id);

        /// <summary>
        /// جلب المدن النشطة فقط (اللي فيها عقارات أو مشاريع)
        /// </summary>
        Task<IEnumerable<City>> GetActiveCitiesAsync();

        /// <summary>
        /// البحث في المدن حسب الاسم
        /// </summary>
        Task<IEnumerable<City>> SearchByNameAsync(string searchTerm);

        /// <summary>
        /// جلب عدد العقارات في مدينة معينة
        /// </summary>
        Task<int> GetPropertiesCountAsync(long cityId);

        /// <summary>
        /// جلب عدد المشاريع في مدينة معينة
        /// </summary>
        Task<int> GetProjectsCountAsync(long cityId);
    }
}
