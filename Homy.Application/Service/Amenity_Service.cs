using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;
using Homy.Infurastructure.Unitofworks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Infurastructure.Service
{
    public class Amenity_Service : IAmenity_Service
    {
        private readonly IAmenity_Repo _amenityRepo;
        private readonly IUnitofwork _unitofwork;

        public Amenity_Service(IAmenity_Repo amenityRepo, IUnitofwork unitofwork)
        {
            _amenityRepo = amenityRepo;
            _unitofwork = unitofwork;
        }

        public async Task<IEnumerable<Homy.Domin.models.Amenity>> GetAllAmenitiesAsync()
        {
            return await _amenityRepo.GetAllAsync();
        }

        public async Task<Homy.Domin.models.Amenity> GetAmenityByIdAsync(int id)
        {
            return await _amenityRepo.GetByIdAsync(id);
        }

        public async Task<Homy.Domin.models.Amenity> CreateAmenityAsync(string name, string? nameEn, string? iconUrl = null)
        {
            var amenity = new Homy.Domin.models.Amenity 
            { 
                Name = name,
                NameEn = nameEn, 
                IconUrl = iconUrl
            };
            await _amenityRepo.AddAsync(amenity);
            await _unitofwork.Save();
            return amenity;
        }

        public async Task<Homy.Domin.models.Amenity> UpdateAmenityAsync(int id, string name, string? nameEn, string? iconUrl = null)
        {
            var amenity = await _amenityRepo.GetByIdAsync(id);
            if (amenity == null) return null;

            amenity.Name = name;
            amenity.NameEn = nameEn; 
            if (!string.IsNullOrEmpty(iconUrl))
            {
                amenity.IconUrl = iconUrl;
            }
            
            _amenityRepo.Update(amenity);
            await _unitofwork.Save();
            return amenity;
        }

        public async Task<bool> DeleteAmenityAsync(int id)
        {
            var amenity = await _amenityRepo.GetByIdAsync(id);
            if (amenity == null) return false;

            await _amenityRepo.DeleteAsync(id);
            await _unitofwork.Save();
            return true;
        }
    }
}