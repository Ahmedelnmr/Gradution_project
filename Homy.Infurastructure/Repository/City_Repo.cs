using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Infurastructure.Repository
{
    public class City_Repo : Genric_Repo<City>, ICity_Repo
    {
        public City_Repo(HomyContext context) : base(context)
        {
        }

   
        public async Task<IEnumerable<City>> GetAllWithDistrictsAsync()
        {
            return await dbset
                .Include(c => c.Districts)           
                .Where(c => !c.IsDeleted)            // استبعاد المحذوف
                .OrderBy(c => c.Name)                // ترتيب أبجدي
                .ToListAsync();
        }

        
        public async Task<City> GetByIdWithDetailsAsync(long id)
        {
            return await dbset
                .Include(c => c.Districts)           // جلب الأحياء
                .Include(c => c.Properties)          // جلب العقارات
                    .ThenInclude(p => p.Images)      // جلب صور العقارات
                .Include(c => c.Projects)            // جلب المشاريع
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

       
        public async Task<IEnumerable<City>> GetActiveCitiesAsync()
        {
            return await dbset
                .Include(c => c.Districts)
                .Where(c => !c.IsDeleted &&
                           (c.Properties.Any(p => !p.IsDeleted) ||
                            c.Projects.Any(pr => !pr.IsDeleted)))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// البحث في المدن حسب الاسم
        /// استخدام Contains للبحث الجزئي (Partial Match)
        /// </summary>
        public async Task<IEnumerable<City>> SearchByNameAsync(string searchTerm)
        {
            // إذا كان البحث فاضي، نرجع كل المدن
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllWithDistrictsAsync();

            return await dbset
                .Include(c => c.Districts)
                .Where(c => c.Name.Contains(searchTerm) && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// جلب عدد العقارات في مدينة معينة
        /// استخدام Count() بدل ToList().Count() علشان Performance
        /// </summary>
        public async Task<int> GetPropertiesCountAsync(long cityId)
        {
            return await context.Properties
                .Where(p => p.CityId == cityId && !p.IsDeleted)
                .CountAsync();
        }

        /// <summary>
        /// جلب عدد المشاريع في مدينة معينة
        /// </summary>
        public async Task<int> GetProjectsCountAsync(long cityId)
        {
            return await context.Projects
                .Where(p => p.CityId == cityId && !p.IsDeleted)
                .CountAsync();
        }
    }
}
