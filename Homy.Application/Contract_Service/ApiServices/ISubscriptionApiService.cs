using Homy.Application.Dtos.ApiDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Application.Contract_Service.ApiServices
{
    public interface ISubscriptionApiService
    {
        /// <summary>
        /// Get all available packages
        /// </summary>
        Task<List<PackageDto>> GetAvailablePackagesAsync();

        /// <summary>
        /// Get user's current subscription status
        /// </summary>
        Task<SubscriptionStatusDto> GetSubscriptionStatusAsync(Guid userId);

        /// <summary>
        /// Create PayPal order for a package
        /// </summary>
        Task<PayPalOrderResponse> CreatePayPalOrderAsync(Guid userId, long packageId);

        /// <summary>
        /// Capture PayPal payment and activate subscription
        /// </summary>
        Task<(bool success, string message)> CapturePaymentAndSubscribeAsync(Guid userId, CapturePayPalPaymentDto payment);

        /// <summary>
        /// Check if user can add a property (validates verification and subscription)
        /// </summary>
        Task<CanAddPropertyDto> CanAddPropertyAsync(Guid userId, bool isFeatured = false);

        /// <summary>
        /// Check and process expiring subscriptions (for background job)
        /// </summary>
        Task ProcessExpiringSubscriptionsAsync();

        /// <summary>
        /// Check and process expired subscriptions (for background job)
        /// </summary>
        Task ProcessExpiredSubscriptionsAsync();
    }
}
