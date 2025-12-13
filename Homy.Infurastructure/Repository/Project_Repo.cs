using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Homy.Infurastructure.Repository
{
    public class Project_Repo : Genric_Repo<Project>, IProject_Repo
    {
        public Project_Repo(HomyContext context) : base(context)
        {

        }

        /// <summary>
        /// جلب كل المشاريع مع التفاصيل
        /// Include: City, District, Properties
        /// </summary>
        public async Task<IEnumerable<Project>> GetAllWithDetailsAsync()
        {
            return await dbset
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Properties)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// جلب مشروع واحد مع كل التفاصيل
        /// </summary>
        public async Task<Project> GetByIdWithDetailsAsync(long id)
        {
            return await dbset
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Properties)
                    .ThenInclude(prop => prop.Images)
                .Include(p => p.Properties)
                    .ThenInclude(prop => prop.PropertyType)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        /// <summary>
        /// جلب المشاريع حسب المدينة
        /// </summary>
        public async Task<IEnumerable<Project>> GetByCityIdAsync(long cityId)
        {
            return await dbset
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Properties)
                .Where(p => p.CityId == cityId && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        /// <summary>
        /// جلب المشاريع حسب الحي
        /// </summary>
        public async Task<IEnumerable<Project>> GetByDistrictIdAsync(long districtId)
        {
            return await dbset
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Properties)
                .Where(p => p.DistrictId == districtId && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        /// <summary>
        /// جلب المشاريع النشطة فقط
        /// </summary>
        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await dbset
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Properties)
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// البحث في المشاريع حسب الاسم
        /// </summary>
        public async Task<IEnumerable<Project>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllWithDetailsAsync();

            return await dbset
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Properties)
                .Where(p => p.Name.Contains(searchTerm) && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        /// <summary>
        /// جلب عدد الوحدات في مشروع معين
        /// </summary>
        public async Task<int> GetPropertiesCountAsync(long projectId)
        {
            return await context.Properties
                .Where(p => p.ProjectId == projectId && !p.IsDeleted)
                .CountAsync();
        }

        /// <summary>
        /// جلب أقل سعر للوحدات في مشروع
        /// </summary>
        public async Task<decimal?> GetMinPriceAsync(long projectId)
        {
            var properties = context.Properties
                .Where(p => p.ProjectId == projectId && !p.IsDeleted);

            if (!await properties.AnyAsync())
                return null;

            return await properties.MinAsync(p => p.Price);
        }

        /// <summary>
        /// جلب المشاريع مع عدد الوحدات
        /// استخدام GroupJoin للحصول على المشاريع حتى لو مافيش وحدات
        /// </summary>
        public async Task<IEnumerable<(Project Project, int PropertiesCount)>> GetProjectsWithCountAsync()
        {
            var result = await dbset
                .Include(p => p.City)
                .Include(p => p.District)
                .Where(p => !p.IsDeleted)
                .Select(p => new
                {
                    Project = p,
                    PropertiesCount = p.Properties.Count(prop => !prop.IsDeleted)
                })
                .OrderByDescending(x => x.PropertiesCount)
                .ToListAsync();

            return result.Select(x => (x.Project, x.PropertiesCount));
        }
    }
}