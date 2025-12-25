using Homy.Application.Contract_Service.ApiServices;
using Homy.Application.Dtos.ApiDtos;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Application.Service.ApiServices
{
    public class NotificationApiService : INotificationApiService
    {
        private readonly IUnitofwork _unitOfWork;

        public NotificationApiService(IUnitofwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<NotificationDto>> GetUserNotificationsAsync(
            Guid userId, 
            int pageNumber = 1, 
            int pageSize = 20, 
            bool? unreadOnly = null)
        {
            IQueryable<Notification> query = _unitOfWork.NotificationRepo.GetAll()
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .Include(n => n.Property); // Include property if exists

            // Filter by read status if specified
            if (unreadOnly.HasValue && unreadOnly.Value)
            {
                query = query.Where(n => !n.IsRead);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination and ordering
            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to DTOs
            var notificationDtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type.ToString(),
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                PropertyId = n.PropertyId,
                PropertyTitle = n.Property?.Title
            }).ToList();

            return new PagedResultDto<NotificationDto>
            {
                Items = notificationDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<bool> MarkAsReadAsync(Guid userId, long notificationId)
        {
            var notification = await _unitOfWork.NotificationRepo.GetAll()
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId && !n.IsDeleted);

            if (notification == null)
                return false;

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                // Assuming NotificationRepo has Update method or using EF tracking
                await _unitOfWork.Save();
            }

            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _unitOfWork.NotificationRepo.GetAll()
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .ToListAsync();

            if (!unreadNotifications.Any())
                return true;

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            await _unitOfWork.Save();
            return true;
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _unitOfWork.NotificationRepo.GetAll()
                .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);
        }
    }
}
