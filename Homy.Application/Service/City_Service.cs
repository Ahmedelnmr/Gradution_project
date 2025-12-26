using Homy.Application.Dtos;
using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;

namespace Homy.Infurastructure.Service
{
    public class City_Service : ICity_Service
    {
        private readonly IUnitofwork _unitofwork;

        public City_Service(IUnitofwork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        /// <summary>
        /// ??? ?? ????? (???? ??????)
        /// </summary>
        public async Task<IEnumerable<City>> GetAllCitiesAsync()
        {
            return await _unitofwork.CityRepo.GetAllAsync();
        }

        /// <summary>
        /// ??? ?? ????? ?? ???????
        /// </summary>
        public async Task<IEnumerable<City>> GetAllCitiesWithDistrictsAsync()
        {
            return await _unitofwork.CityRepo.GetAllWithDistrictsAsync();
        }

        /// <summary>
        /// ??? ????? ?????
        /// </summary>
        public async Task<City> GetCityByIdAsync(int id)
        {
            return await _unitofwork.CityRepo.GetByIdAsync(id);
        }

        /// <summary>
        /// ??? ????? ?? ?? ????????
        /// </summary>
        public async Task<City> GetCityWithDetailsAsync(long id)
        {
            return await _unitofwork.CityRepo.GetByIdWithDetailsAsync(id);
        }

        /// <summary>
        /// ??? ????? ?????? ???
        /// </summary>
        public async Task<IEnumerable<City>> GetActiveCitiesAsync()
        {
            return await _unitofwork.CityRepo.GetActiveCitiesAsync();
        }

        /// <summary>
        /// ????? ?? ?????
        /// </summary>
        public async Task<IEnumerable<City>> SearchCitiesAsync(string searchTerm)
        {
            return await _unitofwork.CityRepo.SearchByNameAsync(searchTerm);
        }

        /// <summary>
        /// ????? ????? ?????
        /// </summary>
        public async Task<City> CreateCityAsync(string name, string? nameEn, Guid? createdById)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("??? ??????? ?????");

            // ?????? ?? ??? ????? ?????
            var existingCities = await _unitofwork.CityRepo.GetAllAsync();
            if (existingCities.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("??????? ?????? ??????");

            var city = new City
            {
                Name = name,
                NameEn = nameEn, 
                CreatedAt = DateTime.Now,
                CreatedById = createdById,
                IsDeleted = false
            };

            var createdCity = await _unitofwork.CityRepo.AddAsync(city);
            await _unitofwork.Save();

            return createdCity;
        }

        /// <summary>
        /// ????? ?????
        /// </summary>
        public async Task<City> UpdateCityAsync(long id, string name, string? nameEn, Guid? updatedById)
        {
            var city = await _unitofwork.CityRepo.GetByIdAsync((int)id);

            if (city == null)
                throw new ArgumentException("??????? ??? ??????");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("??? ??????? ?????");

            city.Name = name;
            city.NameEn = nameEn;
            city.UpdatedAt = DateTime.Now;
            city.UpdatedById = updatedById;

            _unitofwork.CityRepo.Update(city);
            await _unitofwork.Save();

            return city;
        }

        /// <summary>
        /// ??? ????? (Soft Delete)
        /// </summary>
        public async Task<bool> DeleteCityAsync(long id)
        {
            var city = await _unitofwork.CityRepo.GetByIdAsync((int)id);

            if (city == null)
                return false;

            // ?????? ?? ???? ?????? ?? ?????? ??????
            var propertiesCount = await _unitofwork.CityRepo.GetPropertiesCountAsync(id);
            var projectsCount = await _unitofwork.CityRepo.GetProjectsCountAsync(id);

            if (propertiesCount > 0 || projectsCount > 0)
            {
                throw new InvalidOperationException(
                    $"?? ???? ??? ??????? ????? {propertiesCount} ???? ? {projectsCount} ????? ??????? ???"
                );
            }

            city.IsDeleted = true;
            city.UpdatedAt = DateTime.Now;

            _unitofwork.CityRepo.Update(city);
            await _unitofwork.Save();

            return true;
        }

        /// <summary>
        /// ??? ???????? ???????
        /// </summary>
        public async Task<CityStatsDto> GetCityStatsAsync(long cityId)
        {
            var propertiesCount = await _unitofwork.CityRepo.GetPropertiesCountAsync(cityId);
            var projectsCount = await _unitofwork.CityRepo.GetProjectsCountAsync(cityId);

            var city = await _unitofwork.CityRepo.GetByIdWithDetailsAsync(cityId);
            var districtsCount = city?.Districts?.Count ?? 0;

            return new CityStatsDto
            {
                CityId = cityId,
                PropertiesCount = propertiesCount,
                ProjectsCount = projectsCount,
                DistrictsCount = districtsCount
            };
        }
    }
}