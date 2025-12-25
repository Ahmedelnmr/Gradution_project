using Homy.Application.Dtos.ApiDtos;
using System;
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

        // ===== NEW: Property Management Methods =====

        /// <summary>
        /// Create a new property (with subscription validation & image upload)
        /// </summary>
        Task<(bool success, string message, long? propertyId)> CreatePropertyAsync(Guid userId, CreatePropertyDto dto);

        /// <summary>
        /// Update an existing property
        /// </summary>
        Task<(bool success, string message)> UpdatePropertyAsync(Guid userId, long propertyId, UpdatePropertyDto dto);

        /// <summary>
        /// Delete a property (soft delete)
        /// </summary>
        Task<bool> DeletePropertyAsync(Guid userId, long propertyId);

        /// <summary>
        /// Get user's properties with pagination
        /// </summary>
        Task<PagedResultDto<PropertyCardDto>> GetUserPropertiesAsync(Guid userId, PropertyFilterDto filter);

        /// <summary>
        /// Admin: Approve or reject a property
        /// </summary>
        Task<(bool success, string message)> ApproveOrRejectPropertyAsync(PropertyApprovalDto dto);
    }
}
