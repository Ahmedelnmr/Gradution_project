using Homy.Application.Dtos;
using Homy.Application.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Application.Service
{
    public class Package_Service : IPackage_Service
    {
        private readonly IUnitofwork _unitOfWork;

        public Package_Service(IUnitofwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PackageDto>> GetAllPackagesAsync()
        {
            // Use GetAll() IQueryable to include subscriptions
            var packages = await _unitOfWork.PackageRepo.GetAll()
                .Include(p => p.Subscriptions)
                .ToListAsync();
            
            var resultList = new List<PackageDto>();
            foreach (var pkg in packages)
            {
                var dto = pkg.Adapt<PackageDto>();
                dto.SubscriptionsCount = pkg.Subscriptions?.Count ?? 0;
                resultList.Add(dto);
            }
            return resultList;
        }

        public async Task<PackageDto> GetPackageByIdAsync(int id)
        {
            var package = await _unitOfWork.PackageRepo.GetByIdAsync(id);
            return package?.Adapt<PackageDto>();
        }

        public async Task CreatePackageAsync(PackageDto packageDto)
        {
            var package = packageDto.Adapt<Package>();
            await _unitOfWork.PackageRepo.AddAsync(package);
            await _unitOfWork.Save();
        }

        public async Task UpdatePackageAsync(PackageDto packageDto)
        {
            var existingPackage = await _unitOfWork.PackageRepo.GetByIdAsync(packageDto.Id);
            if (existingPackage != null)
            {
                // Update properties
                existingPackage.Name = packageDto.Name;
                existingPackage.Price = packageDto.Price;
                existingPackage.DurationDays = packageDto.DurationDays;
                existingPackage.MaxProperties = packageDto.MaxProperties;
                existingPackage.MaxFeatured = packageDto.MaxFeatured;
                existingPackage.CanBumpUp = packageDto.CanBumpUp;

                _unitOfWork.PackageRepo.Update(existingPackage);
                await _unitOfWork.Save();
            }
        }

        public async Task DeletePackageAsync(int id)
        {
            await _unitOfWork.PackageRepo.DeleteAsync(id);
            await _unitOfWork.Save();
        }
    }
}

