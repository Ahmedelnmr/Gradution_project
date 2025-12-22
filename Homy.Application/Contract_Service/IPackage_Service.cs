using Homy.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Application.Contract_Service
{
    public interface IPackage_Service
    {
        Task<IEnumerable<PackageDto>> GetAllPackagesAsync();
        Task<PackageDto> GetPackageByIdAsync(int id);
        Task CreatePackageAsync(PackageDto packageDto);
        Task UpdatePackageAsync(PackageDto packageDto);
        Task DeletePackageAsync(int id);
    }
}
