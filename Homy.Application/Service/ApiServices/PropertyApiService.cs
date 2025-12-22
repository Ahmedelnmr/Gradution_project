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
    public class PropertyApiService : IPropertyApiService
    {
        private readonly IUnitofwork _unitOfWork;

        public PropertyApiService(IUnitofwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<PropertyListItemDto>> GetPropertiesAsync(PropertyFilterDto filter)
        {
            var query = _unitOfWork.PropertyRepo.GetAll()
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Project)
                .Include(p => p.User)
                .Include(p => p.Images)
                .Where(p => !p.IsDeleted);

            // Filter by Purpose (ForSale=1, ForRent=2, Both=3)
            if (filter.Purpose.HasValue)
            {
                if (filter.Purpose == 1) // ForSale
                {
                    query = query.Where(p => p.Purpose == PropertyPurpose.ForSale || p.Purpose == PropertyPurpose.Both);
                }
                else if (filter.Purpose == 2) // ForRent
                {
                    query = query.Where(p => p.Purpose == PropertyPurpose.ForRent || p.Purpose == PropertyPurpose.Both);
                }
            }

            // Filter by Location
            if (filter.CityId.HasValue)
            {
                query = query.Where(p => p.CityId == filter.CityId.Value);
            }

            if (filter.DistrictId.HasValue)
            {
                query = query.Where(p => p.DistrictId == filter.DistrictId.Value);
            }

            if (filter.ProjectId.HasValue)
            {
                query = query.Where(p => p.ProjectId == filter.ProjectId.Value);
            }

            // Filter by Property Type
            if (filter.PropertyTypeId.HasValue)
            {
                query = query.Where(p => p.PropertyTypeId == filter.PropertyTypeId.Value);
            }

            // Filter by Price (for sale)
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            // Filter by Rent Price
            if (filter.MinRentPrice.HasValue)
            {
                query = query.Where(p => p.RentPriceMonthly >= filter.MinRentPrice.Value);
            }

            if (filter.MaxRentPrice.HasValue)
            {
                query = query.Where(p => p.RentPriceMonthly <= filter.MaxRentPrice.Value);
            }

            // Filter by Area
            if (filter.MinArea.HasValue)
            {
                query = query.Where(p => p.Area >= filter.MinArea.Value);
            }

            if (filter.MaxArea.HasValue)
            {
                query = query.Where(p => p.Area <= filter.MaxArea.Value);
            }

            // Filter by Rooms
            if (filter.MinRooms.HasValue)
            {
                query = query.Where(p => p.Rooms >= filter.MinRooms.Value);
            }

            if (filter.MaxRooms.HasValue)
            {
                query = query.Where(p => p.Rooms <= filter.MaxRooms.Value);
            }

            // Filter by Bathrooms
            if (filter.MinBathrooms.HasValue)
            {
                query = query.Where(p => p.Bathrooms >= filter.MinBathrooms.Value);
            }

            // Filter by Status
            if (filter.Status.HasValue)
            {
                query = query.Where(p => p.Status == (PropertyStatus)filter.Status.Value);
            }
            else
            {
                // Default: only show Active properties
                query = query.Where(p => p.Status == PropertyStatus.Active);
            }

            // Filter by Finishing Type
            if (filter.FinishingType.HasValue)
            {
                query = query.Where(p => p.FinishingType == (FinishingType)filter.FinishingType.Value);
            }

            // Filter by Featured
            if (filter.IsFeatured.HasValue && filter.IsFeatured.Value)
            {
                query = query.Where(p => p.IsFeatured && 
                    (p.FeaturedUntil == null || p.FeaturedUntil > DateTime.UtcNow));
            }

            // Search in Title and Description
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Title.ToLower().Contains(searchTerm) || 
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            query = filter.SortBy?.ToLower() switch
            {
                "price" => filter.SortDescending 
                    ? query.OrderByDescending(p => p.Price) 
                    : query.OrderBy(p => p.Price),
                "area" => filter.SortDescending 
                    ? query.OrderByDescending(p => p.Area) 
                    : query.OrderBy(p => p.Area),
                "featured" => query.OrderByDescending(p => p.IsFeatured)
                    .ThenByDescending(p => p.CreatedAt),
                _ => filter.SortDescending 
                    ? query.OrderByDescending(p => p.CreatedAt) 
                    : query.OrderBy(p => p.CreatedAt)
            };

            // Pagination
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new PropertyListItemDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    RentPriceMonthly = p.RentPriceMonthly,
                    Currency = "EGP",
                    PropertyType = p.PropertyType.Name,
                    PropertyTypeEn = p.PropertyType.NameEn,
                    City = p.City.Name,
                    CityEn = p.City.NameEn,
                    District = p.District != null ? p.District.Name : null,
                    DistrictEn = p.District != null ? p.District.NameEn : null,
                    ProjectName = p.Project != null ? p.Project.Name : null,
                    Area = p.Area,
                    Rooms = p.Rooms,
                    Bathrooms = p.Bathrooms,
                    MainImageUrl = p.Images.OrderBy(i => i.SortOrder).FirstOrDefault(i => i.IsMain) != null
                        ? p.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl
                        : p.Images.OrderBy(i => i.SortOrder).FirstOrDefault() != null
                            ? p.Images.OrderBy(i => i.SortOrder).FirstOrDefault()!.ImageUrl
                            : null,
                    IsFeatured = p.IsFeatured,
                    Status = p.Status.ToString(),
                    Purpose = p.Purpose.ToString(),
                    FinishingType = p.FinishingType.HasValue ? p.FinishingType.Value.ToString() : null,
                    AgentId = p.UserId,
                    AgentName = p.User.FullName,
                    AgentProfileImage = p.User.ProfileImageUrl,
                    CreatedAt = p.CreatedAt,
                    ViewCount = p.ViewCount
                })
                .ToListAsync();

            return new PagedResultDto<PropertyListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<PropertyDetailsDto?> GetPropertyByIdAsync(long id)
        {
            var property = await _unitOfWork.PropertyRepo.GetAll()
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Project)
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Where(p => !p.IsDeleted && p.Id == id)
                .FirstOrDefaultAsync();

            if (property == null)
                return null;

            // Get agent's active properties count
            var agentActivePropertiesCount = await _unitOfWork.PropertyRepo.GetAll()
                .Where(p => p.UserId == property.UserId && 
                           p.Status == PropertyStatus.Active && 
                           !p.IsDeleted)
                .CountAsync();

            return new PropertyDetailsDto
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                RentPriceMonthly = property.RentPriceMonthly,
                Currency = "EGP",
                PropertyTypeId = property.PropertyTypeId,
                PropertyType = property.PropertyType.Name,
                PropertyTypeEn = property.PropertyType.NameEn,
                CityId = property.CityId,
                City = property.City.Name,
                CityEn = property.City.NameEn,
                DistrictId = property.DistrictId,
                District = property.District?.Name,
                DistrictEn = property.District?.NameEn,
                ProjectId = property.ProjectId,
                ProjectName = property.Project?.Name,
                AddressDetails = property.AddressDetails,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                Area = property.Area,
                Rooms = property.Rooms,
                Bathrooms = property.Bathrooms,
                FloorNumber = property.FloorNumber,
                Status = property.Status.ToString(),
                Purpose = property.Purpose.ToString(),
                FinishingType = property.FinishingType.HasValue ? property.FinishingType.Value.ToString() : null,
                IsFeatured = property.IsFeatured,
                FeaturedUntil = property.FeaturedUntil,
                IsAgricultural = property.IsAgricultural,
                Images = property.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new PropertyImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        IsMain = i.IsMain,
                        SortOrder = i.SortOrder
                    })
                    .ToList(),
                Amenities = property.PropertyAmenities
                    .Select(pa => new AmenityDto
                    {
                        Id = pa.Amenity.Id,
                        Name = pa.Amenity.Name,
                        NameEn = pa.Amenity.NameEn,
                        IconUrl = pa.Amenity.IconUrl
                    })
                    .ToList(),
                Agent = new AgentInfoDto
                {
                    Id = property.User.Id,
                    FullName = property.User.FullName,
                    Email = property.User.Email,
                    Phone = property.User.PhoneNumber,
                    WhatsAppNumber = property.User.WhatsAppNumber,
                    ProfileImageUrl = property.User.ProfileImageUrl,
                    IsVerified = property.User.IsVerified,
                    ActivePropertiesCount = agentActivePropertiesCount
                },
                ViewCount = property.ViewCount,
                WhatsAppClicks = property.WhatsAppClicks,
                PhoneClicks = property.PhoneClicks,
                CreatedAt = property.CreatedAt,
                UpdatedAt = property.UpdatedAt
            };
        }

        public async Task IncrementViewCountAsync(long propertyId)
        {
            var property = await _unitOfWork.PropertyRepo.GetByIdAsync(propertyId);
            if (property != null && !property.IsDeleted)
            {
                property.ViewCount++;
                _unitOfWork.PropertyRepo.Update(property);
                await _unitOfWork.Save();
            }
        }

        public async Task IncrementWhatsAppClickAsync(long propertyId)
        {
            var property = await _unitOfWork.PropertyRepo.GetByIdAsync(propertyId);
            if (property != null && !property.IsDeleted)
            {
                property.WhatsAppClicks++;
                _unitOfWork.PropertyRepo.Update(property);
                await _unitOfWork.Save();
            }
        }

        public async Task IncrementPhoneClickAsync(long propertyId)
        {
            var property = await _unitOfWork.PropertyRepo.GetByIdAsync(propertyId);
            if (property != null && !property.IsDeleted)
            {
                property.PhoneClicks++;
                _unitOfWork.PropertyRepo.Update(property);
                await _unitOfWork.Save();
            }
        }
    }
}
