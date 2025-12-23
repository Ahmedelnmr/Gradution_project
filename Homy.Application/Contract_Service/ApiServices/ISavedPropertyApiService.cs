using Homy.Application.Dtos.ApiDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Application.Contract_Service.ApiServices
{
    public interface ISavedPropertyApiService
    {
        /// <summary>
        /// Get all saved properties for a user
        /// </summary>
        Task<List<SavedPropertyDto>> GetUserSavedPropertiesAsync(Guid userId);
        
        /// <summary>
        /// Save a property for a user
        /// </summary>
        Task<bool> SavePropertyAsync(Guid userId, long propertyId);
        
        /// <summary>
        /// Unsave (remove) a property for a user
        /// </summary>
        Task<bool> UnsavePropertyAsync(Guid userId, long propertyId);
        
        /// <summary>
        /// Check if a property is saved by a user
        /// </summary>
        Task<bool> IsPropertySavedAsync(Guid userId, long propertyId);
        
        /// <summary>
        /// Toggle save/unsave property (if saved → unsave, if not saved → save)
        /// </summary>
        /// <returns>True if property is now saved, False if unsaved</returns>
        Task<bool> ToggleSavePropertyAsync(Guid userId, long propertyId);
    }
}
