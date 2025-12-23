using Homy.Application.Contract_Service.ApiServices;
using Homy.Application.Dtos.ApiDtos;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Application.Service.ApiServices
{
    public class SavedPropertyApiService : ISavedPropertyApiService
    {
        private readonly IUnitofwork _unitOfWork;

        public SavedPropertyApiService(IUnitofwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SavedPropertyDto>> GetUserSavedPropertiesAsync(Guid userId)
        {
            var savedProperties = await _unitOfWork.SavedPropertyRepo.GetAll()
                .Where(sp => sp.UserId == userId && !sp.IsDeleted)
                .Include(sp => sp.Property)
                    .ThenInclude(p => p.PropertyType)
                .Include(sp => sp.Property)
                    .ThenInclude(p => p.City)
                .Include(sp => sp.Property)
                    .ThenInclude(p => p.District)
                .Include(sp => sp.Property)
                    .ThenInclude(p => p.Project)
                .Include(sp => sp.Property)
                    .ThenInclude(p => p.Images)
                .Include(sp => sp.Property)
                    .ThenInclude(p => p.User)
                .Where(sp => !sp.Property.IsDeleted && sp.Property.Status == PropertyStatus.Active)
                .OrderByDescending(sp => sp.CreatedAt)
                .Select(sp => new SavedPropertyDto
                {
                    SavedPropertyId = sp.Id,
                    SavedAt = sp.CreatedAt,
                    Property = new PropertyListItemDto
                    {
                        Id = sp.Property.Id,
                        Title = sp.Property.Title,
                        Price = sp.Property.Price,
                        RentPriceMonthly = sp.Property.RentPriceMonthly,
                        Currency = "EGP",
                        PropertyType = sp.Property.PropertyType.Name,
                        PropertyTypeEn = sp.Property.PropertyType.NameEn,
                        City = sp.Property.City.Name,
                        CityEn = sp.Property.City.NameEn,
                        District = sp.Property.District != null ? sp.Property.District.Name : null,
                        DistrictEn = sp.Property.District != null ? sp.Property.District.NameEn : null,
                        ProjectName = sp.Property.Project != null ? sp.Property.Project.Name : null,
                        Area = sp.Property.Area,
                        Rooms = sp.Property.Rooms,
                        Bathrooms = sp.Property.Bathrooms,
                        MainImageUrl = sp.Property.Images.OrderBy(i => i.SortOrder).FirstOrDefault(i => i.IsMain) != null
                            ? sp.Property.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl
                            : sp.Property.Images.OrderBy(i => i.SortOrder).FirstOrDefault() != null
                                ? sp.Property.Images.OrderBy(i => i.SortOrder).FirstOrDefault()!.ImageUrl
                                : null,
                        IsFeatured = sp.Property.IsFeatured,
                        Status = sp.Property.Status.ToString(),
                        Purpose = sp.Property.Purpose.ToString(),
                        FinishingType = sp.Property.FinishingType.HasValue ? sp.Property.FinishingType.Value.ToString() : null,
                        AgentId = sp.Property.UserId,
                        AgentName = sp.Property.User.FullName,
                        AgentProfileImage = sp.Property.User.ProfileImageUrl,
                        CreatedAt = sp.Property.CreatedAt,
                        ViewCount = sp.Property.ViewCount
                    }
                })
                .ToListAsync();

            return savedProperties;
        }

        public async Task<bool> SavePropertyAsync(Guid userId, long propertyId)
        {
            // Check if property exists and is active
            var property = await _unitOfWork.PropertyRepo.GetByIdAsync(propertyId);
            if (property == null || property.IsDeleted || property.Status != PropertyStatus.Active)
                return false;

            // Check if already saved (including soft deleted)
            var existingSaved = await _unitOfWork.SavedPropertyRepo.GetAll()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.PropertyId == propertyId);

            if (existingSaved != null)
            {
                // If it was soft deleted, restore it
                if (existingSaved.IsDeleted)
                {
                    existingSaved.IsDeleted = false;
                    existingSaved.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.SavedPropertyRepo.Update(existingSaved);
                    await _unitOfWork.Save();
                    return true;
                }
                
                // Already saved
                return true;
            }

            // Create new saved property
            var savedProperty = new SavedProperty
            {
                UserId = userId,
                PropertyId = propertyId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SavedPropertyRepo.AddAsync(savedProperty);
            await _unitOfWork.Save();
            
            return true;
        }

        public async Task<bool> UnsavePropertyAsync(Guid userId, long propertyId)
        {
            var savedProperty = await _unitOfWork.SavedPropertyRepo.GetAll()
                .IgnoreQueryFilters() // Important to find it even if filter logic changes, though IsDeleted filter usually hides deleted ones
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.PropertyId == propertyId);

            if (savedProperty == null || (savedProperty.IsDeleted))
                return false; 

            // Soft delete
            savedProperty.IsDeleted = true;
            savedProperty.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.SavedPropertyRepo.Update(savedProperty);
            await _unitOfWork.Save();
            
            return true;
        }

        public async Task<bool> IsPropertySavedAsync(Guid userId, long propertyId)
        {
            // Here we only care if it is ACTIVELY saved, so we respect the filter (or explicitly check !IsDeleted)
            return await _unitOfWork.SavedPropertyRepo.GetAll()
                .AnyAsync(sp => sp.UserId == userId && sp.PropertyId == propertyId && !sp.IsDeleted);
        }

        public async Task<bool> ToggleSavePropertyAsync(Guid userId, long propertyId)
        {
            // Check if property exists and is active
            var property = await _unitOfWork.PropertyRepo.GetByIdAsync(propertyId);
            if (property == null || property.IsDeleted || property.Status != PropertyStatus.Active)
                return false;

            // Check current saved status (including soft deleted)
            var existingSaved = await _unitOfWork.SavedPropertyRepo.GetAll()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.PropertyId == propertyId);

            if (existingSaved != null && !existingSaved.IsDeleted)
            {
                // Currently saved → Unsave it
                existingSaved.IsDeleted = true;
                existingSaved.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.SavedPropertyRepo.Update(existingSaved);
                await _unitOfWork.Save();
                return false; // Now unsaved
            }
            else
            {
                // Not saved or was deleted → Save it
                if (existingSaved != null)
                {
                    // Restore previously deleted
                    existingSaved.IsDeleted = false;
                    existingSaved.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.SavedPropertyRepo.Update(existingSaved);
                }
                else
                {
                    // Create new
                    var savedProperty = new SavedProperty
                    {
                        UserId = userId,
                        PropertyId = propertyId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.SavedPropertyRepo.AddAsync(savedProperty);
                }
                
                await _unitOfWork.Save();
                return true; // Now saved
            }
        }
    }
}
