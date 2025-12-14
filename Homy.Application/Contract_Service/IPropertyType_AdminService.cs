using Homy.Application.Dtos.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Domin.Contract_Service
{
    public interface IPropertyType_AdminService
    {
        Task<IEnumerable<PropertyTypeDto>> GetAllAsync();
        Task<PropertyTypeDto?> GetByIdAsync(long id);
        Task<PropertyTypeDto> CreateAsync(CreatePropertyTypeDto dto);
        Task<bool> UpdateAsync(UpdatePropertyTypeDto dto);
        Task<(bool Success, string Message)> DeleteAsync(long id);
    }
}
