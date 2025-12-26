using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;

namespace Homy.Infurastructure.Service
{
    public class District_Service : IDistrict_Service
    {
        private readonly IUnitofwork _unitofwork;

        public District_Service(IUnitofwork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public async Task<IEnumerable<District>> GetAllDistrictsAsync()
        {
            return await _unitofwork.DistrictRepo.GetAllAsync();
        }

        public async Task<District> GetDistrictByIdAsync(int id)
        {
            return await _unitofwork.DistrictRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<District>> GetDistrictsByCityIdAsync(long cityId)
        {
            // TODO: يمكن إضافة method في Repository للفلترة حسب المدينة
            var allDistricts = await _unitofwork.DistrictRepo.GetAllAsync();
            return allDistricts.Where(d => d.CityId == cityId).ToList();
        }

        public async Task<District> CreateDistrictAsync(string name, string? nameEn, long cityId)
        {
            var district = new District { Name = name, NameEn = nameEn, CityId = cityId };
            await _unitofwork.DistrictRepo.AddAsync(district);
            await _unitofwork.Save();
            return district;
        }

        public async Task<District> UpdateDistrictAsync(int id, string name, string? nameEn, long cityId)
        {
            var district = await _unitofwork.DistrictRepo.GetByIdAsync(id);
            if (district == null) return null;

            district.Name = name;
            district.NameEn = nameEn;
            district.CityId = cityId;
            
            _unitofwork.DistrictRepo.Update(district);
            await _unitofwork.Save();
            return district;
        }

        public async Task<bool> DeleteDistrictAsync(int id)
        {
            var district = await _unitofwork.DistrictRepo.GetByIdAsync(id);
            if (district == null) return false;

            await _unitofwork.DistrictRepo.DeleteAsync(id);
            await _unitofwork.Save();
            return true;
        }
    }
}
