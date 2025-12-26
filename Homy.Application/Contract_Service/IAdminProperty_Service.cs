using Homy.Application.Dtos.Admin;
using Homy.Application.Dtos.Common;
using Homy.Domin.models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Domin.Contract_Service
{
    public interface IAdminProperty_Service
    {
        Task<PaginatedResult<PropertyListDto>> GetPropertiesAsync(PropertyFilterDto filter);
        Task<PropertyDetailsDto?> GetPropertyDetailsAsync(long id);
        Task<UpdatePropertyDto?> GetPropertyForEditAsync(long id);
        Task<bool> ReviewPropertyAsync(Guid adminId, CreatePropertyReviewDto dto);
        Task<Dictionary<PropertyStatus, int>> GetStatusCountsAsync();
        
        // CRUD
        Task<Property> CreatePropertyAsync(CreatePropertyDto dto, Guid userId);
        Task<Property> UpdatePropertyAsync(UpdatePropertyDto dto, Guid userId);
        Task<bool> DeletePropertyAsync(long id);
    }
}
