using Homy.Domin.models;

namespace Homy.Domin.Contract_Service
{
    public interface IDistrict_Service
    {
        Task<IEnumerable<District>> GetAllDistrictsAsync();
        Task<District> GetDistrictByIdAsync(int id);
        Task<IEnumerable<District>> GetDistrictsByCityIdAsync(long cityId);
        
        // CRUD
        Task<District> CreateDistrictAsync(string name, string? nameEn, long cityId);
        Task<District> UpdateDistrictAsync(int id, string name, string? nameEn, long cityId);
        Task<bool> DeleteDistrictAsync(int id);
    }
}