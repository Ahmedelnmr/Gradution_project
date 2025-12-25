using Homy.Application.Dtos.ApiDtos;
using System;
using System.Threading.Tasks;

namespace Homy.Application.Contract_Service.ApiServices
{
    public interface INotificationApiService
    {
        /// <summary>
        /// Get paginated notifications for a user
        /// </summary>
        Task<PagedResultDto<NotificationDto>> GetUserNotificationsAsync(Guid userId, int pageNumber = 1, int pageSize = 20, bool? unreadOnly = null);

        /// <summary>
        /// Mark notification as read
        /// </summary>
        Task<bool> MarkAsReadAsync(Guid userId, long notificationId);

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        Task<bool> MarkAllAsReadAsync(Guid userId);

        /// <summary>
        /// Get unread notification count
        /// </summary>
        Task<int> GetUnreadCountAsync(Guid userId);
    }
}
