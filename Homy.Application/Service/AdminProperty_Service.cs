using Homy.Application.Dtos.Admin;
using Homy.Application.Dtos.Common;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Application.Service
{
    public class AdminProperty_Service : IAdminProperty_Service
    {
        private readonly IUnitofwork _unitOfWork;

        public AdminProperty_Service(IUnitofwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<PropertyListDto>> GetPropertiesAsync(PropertyFilterDto filter)
        {
            var query = _unitOfWork.PropertyRepo.GetAll()
                .Include(p => p.User)
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.PropertyType)
                .Include(p => p.Images)
                .AsQueryable();

            // Apply Filters
            if (filter.Status.HasValue)
                query = query.Where(p => p.Status == filter.Status.Value);
            
            if (filter.Purpose.HasValue)
                query = query.Where(p => p.Purpose == filter.Purpose.Value);

            if (filter.PropertyTypeId.HasValue)
                query = query.Where(p => p.PropertyTypeId == filter.PropertyTypeId.Value);

            if (filter.CityId.HasValue)
                query = query.Where(p => p.CityId == filter.CityId.Value);

            if (filter.UserId.HasValue)
                query = query.Where(p => p.UserId == filter.UserId.Value);

            if (filter.ProjectId.HasValue)
                query = query.Where(p => p.ProjectId == filter.ProjectId.Value);

            if (!string.IsNullOrEmpty(filter.Search))
            {
                var search = filter.Search.ToLower();
                query = query.Where(p => 
                    p.Title.ToLower().Contains(search) || 
                    (p.Description != null && p.Description.ToLower().Contains(search)) ||
                    p.User.FullName.ToLower().Contains(search));
            }

            // Pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // Mapster Mapping
            var dtos = items.Adapt<List<PropertyListDto>>();

            return new PaginatedResult<PropertyListDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<PropertyDetailsDto?> GetPropertyDetailsAsync(long id)
        {
            var property = await _unitOfWork.PropertyRepo.GetAll()
                .Include(p => p.User)
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.PropertyType)
                .Include(p => p.Images)
                .Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null) return null;

            var reviews = await _unitOfWork.PropertyReviewRepo.GetByPropertyIdAsync(id);
            
            // Mapster Mapping
            var dto = property.Adapt<PropertyDetailsDto>();
            dto.ReviewHistory = reviews.Adapt<List<PropertyReviewHistoryDto>>();

            return dto;
        }

        public async Task<bool> ReviewPropertyAsync(Guid adminId, CreatePropertyReviewDto dto)
        {
            var property = await _unitOfWork.PropertyRepo.GetByIdAsync((int)dto.PropertyId);
            if (property == null) return false;

            // Update Status based on Action
            switch (dto.Action)
            {
                case ReviewAction.Approved:
                    property.Status = PropertyStatus.Active;
                    break;
                case ReviewAction.Rejected:
                    property.Status = PropertyStatus.Rejected;
                    break;
                case ReviewAction.ChangesRequested:
                    property.Status = PropertyStatus.PendingReview;
                    break;
            }

            // Create Review Log
            var review = new PropertyReview
            {
                PropertyId = dto.PropertyId,
                AdminId = adminId,
                Action = dto.Action,
                Message = dto.Message,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.PropertyReviewRepo.AddAsync(review);

            // Create User Notification
            var notification = new Notification
            {
                UserId = property.UserId,
                PropertyId = property.Id,
                Type = MapActionToNotificationType(dto.Action),
                Title = GetNotificationTitle(dto.Action),
                Message = dto.Message,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.NotificationRepo.AddAsync(notification);
            
            _unitOfWork.PropertyRepo.Update(property);
            await _unitOfWork.Save();

            return true;
        }

        public async Task<Dictionary<PropertyStatus, int>> GetStatusCountsAsync()
        {
            var counts = await _unitOfWork.PropertyRepo.GetAll()
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
            
            // Ensure all statuses exist in dictionary
            foreach (PropertyStatus status in Enum.GetValues(typeof(PropertyStatus)))
            {
                if (!counts.ContainsKey(status))
                {
                    counts[status] = 0;
                }
            }

            return counts;
        }

        // --- Helper Methods ---

        private NotificationType MapActionToNotificationType(ReviewAction action)
        {
            return action switch
            {
                ReviewAction.Approved => NotificationType.PropertyApproved,
                ReviewAction.Rejected => NotificationType.PropertyRejected,
                ReviewAction.ChangesRequested => NotificationType.PropertyChangesRequested,
                _ => NotificationType.General
            };
        }

        private string GetNotificationTitle(ReviewAction action)
        {
             return action switch
            {
                ReviewAction.Approved => "مبروك! تم قبول إعلانك",
                ReviewAction.Rejected => "عذراً، تم رفض إعلانك",
                ReviewAction.ChangesRequested => "تنبيه: إعلانك يحتاج لتعديلات",
                _ => "لديك إشعار جديد"
            };
        }
    }
}
