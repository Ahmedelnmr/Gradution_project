using Homy.Application.Dtos.ApiDtos;
using System.Threading.Tasks;

namespace Homy.Application.Contract_Service.ApiServices
{
    public interface IPropertyApiService
    {
        /// <summary>
        /// Get paginated and filtered properties list
        /// </summary>
        Task<PagedResultDto<PropertyListItemDto>> GetPropertiesAsync(PropertyFilterDto filter);
        
        /// <summary>
        /// Get complete property details by ID
        /// </summary>
        Task<PropertyDetailsDto?> GetPropertyByIdAsync(long id);
        
        /// <summary>
        /// Increment property view count
        /// </summary>
        Task IncrementViewCountAsync(long propertyId);
        
        /// <summary>
        /// Increment WhatsApp click count
        /// </summary>
        Task IncrementWhatsAppClickAsync(long propertyId);
        
        /// <summary>
        /// Increment phone click count
        /// </summary>
        Task IncrementPhoneClickAsync(long propertyId);
    }
}
