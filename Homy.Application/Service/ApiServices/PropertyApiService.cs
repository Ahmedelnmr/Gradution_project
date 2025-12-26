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
        private readonly IFileUploadService _fileUploadService;

        public PropertyApiService(IUnitofwork unitOfWork, IFileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
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

        // ===== Property Management Methods =====

        public async Task<(bool success, string message, long? propertyId)> CreatePropertyAsync(Guid userId, CreatePropertyDto dto)
        {
            var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);
            if (user == null || !user.IsVerified)
                return (false, "المستخدم غير موثق. يجب توثيق حسابك أولاً لإضافة إعلانات.", null);

            var activeSubscription = await _unitOfWork.UserSubscriptionRepo.GetAll()
                .Include(s => s.Package)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive && !s.IsDeleted && s.EndDate > DateTime.UtcNow);

            if (activeSubscription == null)
                return (false, "ليس لديك اشتراك فعال. يرجى الاشتراك في باقة أولاً.", null);

            var userPropertiesCount = await _unitOfWork.PropertyRepo.GetAll()
                .Where(p => p.UserId == userId && !p.IsDeleted).CountAsync();

            if (userPropertiesCount >= activeSubscription.Package.MaxProperties)
                return (false, $"لقد وصلت للحد الأقصى ({activeSubscription.Package.MaxProperties} إعلانات).", null);

            if (dto.IsFeatured)
            {
                var featuredCount = await _unitOfWork.PropertyRepo.GetAll()
                    .Where(p => p.UserId == userId && !p.IsDeleted && p.IsFeatured).CountAsync();
                if (featuredCount >= activeSubscription.Package.MaxFeatured)
                    return (false, $"لقد وصلت للحد الأقصى من الإعلانات المميزة.", null);
            }

            var property = new Property
            {
                Title = dto.Title,
                TitleEn = dto.TitleEn,
                Description = dto.Description,
                DescriptionEn = dto.DescriptionEn,
                AddressDetails = dto.AddressDetails,
                AddressDetailsEn = dto.AddressDetailsEn,
                PropertyTypeId = dto.PropertyTypeId,
                CityId = dto.CityId,
                DistrictId = dto.DistrictId,
                ProjectId = dto.ProjectId,
                Price = dto.Price,
                RentPriceMonthly = dto.RentPriceMonthly,
                Rooms = (byte?)dto.Rooms,
                Bathrooms = (byte?)dto.Bathrooms,
                Area = dto.Area.HasValue ? (int)dto.Area.Value : null,
                FinishingType = dto.FinishingType,
                FloorNumber = (byte?)dto.FloorNumber,
                Purpose = dto.Purpose,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsAgricultural = dto.IsAgricultural,
                IsFeatured = dto.IsFeatured,
                Status = PropertyStatus.PendingReview,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                CreatedById = userId
            };

            await _unitOfWork.PropertyRepo.AddAsync(property);
            await _unitOfWork.Save();

            // Upload images using FileUploadService
            if (dto.Images != null && dto.Images.Any())
            {
                var uploadedUrls = await _fileUploadService.UploadImagesAsync(dto.Images, $"properties/{property.Id}");
                
                for (int i = 0; i < uploadedUrls.Count && i < 6; i++)
                {
                    var propertyImage = new PropertyImage
                    {
                        PropertyId = property.Id,
                        ImageUrl = uploadedUrls[i],
                        IsMain = i == 0,
                        IsPrimary = i == dto.PrimaryImageIndex,
                        SortOrder = i,
                        CreatedAt = DateTime.UtcNow,
                        CreatedById = userId
                    };
                    await _unitOfWork.PropertyImageRepo.AddAsync(propertyImage);
                }
                await _unitOfWork.Save();
            }


            return (true, "تم رفع الإعلان بنجاح! سيتم مراجعته من قبل الإدارة.", property.Id);
        }

        public async Task<(bool success, string message)> UpdatePropertyAsync(Guid userId, long propertyId, UpdatePropertyDto dto)
        {
            var property = await _unitOfWork.PropertyRepo.GetAll()
                .FirstOrDefaultAsync(p => p.Id == propertyId && p.UserId == userId && !p.IsDeleted);

            if (property == null)
                return (false, "الإعلان غير موجود أو ليس لديك صلاحية لتعديله");

            // Update bilingual fields
            property.Title = dto.Title;
            property.TitleEn = dto.TitleEn;
            property.Description = dto.Description;
            property.DescriptionEn = dto.DescriptionEn;
            property.AddressDetails = dto.AddressDetails;
            property.AddressDetailsEn = dto.AddressDetailsEn;

            // Update other fields
            property.PropertyTypeId = dto.PropertyTypeId;
            property.CityId = dto.CityId;
            property.DistrictId = dto.DistrictId;
            property.ProjectId = dto.ProjectId;
            property.Price = dto.Price;
            property.RentPriceMonthly = dto.RentPriceMonthly;
            property.Rooms = (byte?)dto.Rooms;
            property.Bathrooms = (byte?)dto.Bathrooms;
            property.Area = dto.Area.HasValue ? (int)dto.Area.Value : null;
            property.FinishingType = dto.FinishingType;
            property.FloorNumber = (byte?)dto.FloorNumber;
            property.Purpose = dto.Purpose;
            property.Latitude = dto.Latitude;
            property.Longitude = dto.Longitude;
            property.IsAgricultural = dto.IsAgricultural;
            property.UpdatedAt = DateTime.UtcNow;
            property.UpdatedById = userId;

            // If rejected, set back to pending review
            if (property.Status == PropertyStatus.Rejected)
            {
                property.Status = PropertyStatus.PendingReview;
                property.RejectionReason = null;
            }

            _unitOfWork.PropertyRepo.Update(property);
            await _unitOfWork.Save();

            return (true, "تم تحديث الإعلان بنجاح");
        }

        public async Task<bool> DeletePropertyAsync(Guid userId, long propertyId)
        {
            var property = await _unitOfWork.PropertyRepo.GetAll()
                .FirstOrDefaultAsync(p => p.Id == propertyId && p.UserId == userId && !p.IsDeleted);

            if (property == null)
                return false;

            property.IsDeleted = true;
            property.UpdatedAt = DateTime.UtcNow;
            property.UpdatedById = userId;

            _unitOfWork.PropertyRepo.Update(property);
            await _unitOfWork.Save();

            return true;
        }

        public async Task<PagedResultDto<PropertyCardDto>> GetUserPropertiesAsync(Guid userId, PropertyFilterDto filter)
        {
            var query = _unitOfWork.PropertyRepo.GetAll()
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Images)
                .Where(p => p.UserId == userId && !p.IsDeleted);

            // Apply status filter if provided
            if (filter.Status.HasValue)
                query = query.Where(p => p.Status == (PropertyStatus)filter.Status.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new PropertyCardDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    TitleEn = p.TitleEn,
                    Price = p.Price,
                    PropertyType = p.PropertyType.Name,
                    PropertyTypeEn = p.PropertyType.NameEn,
                    City = p.City.Name,
                    CityEn = p.City.NameEn,
                    District = p.District != null ? p.District.Name : null,
                    PrimaryImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary) != null
                        ? p.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl
                        : p.Images.OrderBy(i => i.SortOrder).FirstOrDefault() != null
                            ? p.Images.OrderBy(i => i.SortOrder).FirstOrDefault()!.ImageUrl
                            : null,
                    Purpose = p.Purpose,
                    Status = p.Status,
                    Rooms = p.Rooms,
                    Bathrooms = p.Bathrooms,
                    Area = p.Area,
                    IsFeatured = p.IsFeatured,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return new PagedResultDto<PropertyCardDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<(bool success, string message)> ApproveOrRejectPropertyAsync(Guid adminId, PropertyApprovalDto dto)
        {
            var property = await _unitOfWork.PropertyRepo.GetAll()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == dto.PropertyId && !p.IsDeleted);

            if (property == null)
                return (false, "الإعلان غير موجود");

            property.Status = dto.IsApproved ? PropertyStatus.Active : PropertyStatus.Rejected;
            property.RejectionReason = dto.IsApproved ? null : dto.RejectionReason;
            
            // Create Notification
            var notification = new Notification
            {
                UserId = property.UserId,
                Title = dto.IsApproved ? "✅ تم قبول الإعلان" : "❌ تم رفض الإعلان",
                Message = dto.IsApproved 
                    ? $"تم قبول إعلان \"{property.Title}\" وهو الآن منشور."
                    : $"تم رفض إعلان \"{property.Title}\". السبب: {dto.RejectionReason}",
                Type = dto.IsApproved ? NotificationType.PropertyApproved : NotificationType.PropertyRejected,
                PropertyId = property.Id,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.NotificationRepo.AddAsync(notification);

            // Create PropertyReview Log
            var review = new PropertyReview
            {
                PropertyId = property.Id,
                AdminId = adminId,
                Action = dto.IsApproved ? ReviewAction.Approved : ReviewAction.Rejected,
                Message = dto.IsApproved ? "Approved by admin" : dto.RejectionReason,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.PropertyReviewRepo.AddAsync(review);
            
            _unitOfWork.PropertyRepo.Update(property);
            await _unitOfWork.Save();

            return (true, dto.IsApproved ? "تم قبول الإعلان" : "تم رفض الإعلان");
        }
    }
}
