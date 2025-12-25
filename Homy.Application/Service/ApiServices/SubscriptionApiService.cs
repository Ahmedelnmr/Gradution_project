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
    public class SubscriptionApiService : ISubscriptionApiService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IPayPalService _payPalService;

        public SubscriptionApiService(IUnitofwork unitOfWork, IPayPalService payPalService)
        {
            _unitOfWork = unitOfWork;
            _payPalService = payPalService;
        }

        public async Task<List<PackageDto>> GetAvailablePackagesAsync()
        {
            var packages = await _unitOfWork.PackageRepo.GetAll()
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Price)
                .ToListAsync();

            return packages.Select(p => new PackageDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                DurationDays = p.DurationDays,
                MaxProperties = p.MaxProperties,
                MaxFeatured = p.MaxFeatured,
                CanBumpUp = p.CanBumpUp
            }).ToList();
        }

        public async Task<SubscriptionStatusDto> GetSubscriptionStatusAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);
            if (user == null)
                return new SubscriptionStatusDto { HasActiveSubscription = false };

            // Get active subscription
            var subscription = await _unitOfWork.UserSubscriptionRepo.GetAll()
                .Include(s => s.Package)
                .Where(s => s.UserId == userId && s.IsActive && !s.IsDeleted && s.EndDate > DateTime.UtcNow)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                return new SubscriptionStatusDto { HasActiveSubscription = false };
            }

            // Count user's properties
            var propertiesCount = await _unitOfWork.PropertyRepo.GetAll()
                .CountAsync(p => p.UserId == userId && !p.IsDeleted);

            var featuredCount = await _unitOfWork.PropertyRepo.GetAll()
                .CountAsync(p => p.UserId == userId && !p.IsDeleted && p.IsFeatured);

            return new SubscriptionStatusDto
            {
                HasActiveSubscription = true,
                PackageName = subscription.Package.Name,
                PackagePrice = subscription.Package.Price,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                DaysRemaining = (int)(subscription.EndDate - DateTime.UtcNow).TotalDays,
                PropertiesUsed = propertiesCount,
                PropertiesLimit = subscription.Package.MaxProperties,
                FeaturedUsed = featuredCount,
                FeaturedLimit = subscription.Package.MaxFeatured
            };
        }

        public async Task<PayPalOrderResponse> CreatePayPalOrderAsync(Guid userId, long packageId)
        {
            var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Check if verified
            if (!user.IsVerified)
                throw new InvalidOperationException("يجب توثيق حسابك قبل الاشتراك");

            // Check if already has active subscription
            var existingSubscription = await _unitOfWork.UserSubscriptionRepo.GetAll()
                .AnyAsync(s => s.UserId == userId && s.IsActive && !s.IsDeleted && s.EndDate > DateTime.UtcNow);

            if (existingSubscription)
                throw new InvalidOperationException("لديك اشتراك نشط بالفعل");

            // Get package
            var package = await _unitOfWork.PackageRepo.GetAll()
                .FirstOrDefaultAsync(p => p.Id == packageId && !p.IsDeleted);

            if (package == null)
                throw new InvalidOperationException("الباقة غير موجودة");

            // Create PayPal order
            var (orderId, approvalUrl) = await _payPalService.CreateOrderAsync(
                package.Price,
                "USD",
                $"Homy Subscription - {package.Name} ({package.DurationDays} days)"
            );

            return new PayPalOrderResponse
            {
                OrderId = orderId,
                ApprovalUrl = approvalUrl
            };
        }

        public async Task<(bool success, string message)> CapturePaymentAndSubscribeAsync(Guid userId, CapturePayPalPaymentDto payment)
        {
            var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            // Verify checks again
            if (!user.IsVerified)
                return (false, "يجب توثيق حسابك قبل الاشتراك");

            // Get package
            var package = await _unitOfWork.PackageRepo.GetAll()
                .FirstOrDefaultAsync(p => p.Id == payment.PackageId && !p.IsDeleted);

            if (package == null)
                return (false, "الباقة غير موجودة");

            // Capture PayPal payment
            var (captureSuccess, transactionId, payerId, errorMessage) = await _payPalService.CaptureOrderAsync(payment.PayPalOrderId);

            if (!captureSuccess)
            {
                // Create failed payment notification
                var failedNotification = new Notification
                {
                    UserId = userId,
                    Title = "❌ فشل الدفع",
                    Message = $"لم تتم عملية الدفع بنجاح. السبب: {errorMessage}",
                    Type = NotificationType.PaymentFailed,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.NotificationRepo.AddAsync(failedNotification);
                await _unitOfWork.Save();

                return (false, $"فشل في إتمام عملية الدفع: {errorMessage}");
            }

            // Create subscription
            var subscription = new UserSubscription
            {
                UserId = userId,
                PackageId = payment.PackageId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(package.DurationDays),
                IsActive = true,
                PaymentMethod = "PayPal",
                AmountPaid = package.Price,
                PayPalTransactionId = transactionId,
                PayPalPayerId = payerId ?? payment.PayPalPayerId,
                PaymentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserSubscriptionRepo.AddAsync(subscription);

            // Create success notification
            var successNotification = new Notification
            {
                UserId = userId,
                Title = "✅ تم الاشتراك بنجاح",
                Message = $"مبروك! تم اشتراكك في باقة {package.Name} لمدة {package.DurationDays} يوم. يمكنك الآن إضافة {package.MaxProperties} إعلان و {package.MaxFeatured} إعلان مميز.",
                Type = NotificationType.SubscriptionCreated,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.NotificationRepo.AddAsync(successNotification);

            await _unitOfWork.Save();

            return (true, $"تم الاشتراك بنجاح في باقة {package.Name}");
        }

        public async Task<CanAddPropertyDto> CanAddPropertyAsync(Guid userId, bool isFeatured = false)
        {
            var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);
            if (user == null)
                return new CanAddPropertyDto { CanAdd = false, ErrorMessage = "المستخدم غير موجود", ErrorCode = "USER_NOT_FOUND" };

            // Check verification
            if (!user.IsVerified)
                return new CanAddPropertyDto { CanAdd = false, ErrorMessage = "يجب توثيق حسابك أولاً", ErrorCode = "NOT_VERIFIED" };

            // Get active subscription
            var subscription = await _unitOfWork.UserSubscriptionRepo.GetAll()
                .Include(s => s.Package)
                .Where(s => s.UserId == userId && s.IsActive && !s.IsDeleted && s.EndDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return new CanAddPropertyDto { CanAdd = false, ErrorMessage = "يجب الاشتراك في باقة أولاً", ErrorCode = "NO_SUBSCRIPTION" };

            // Count properties
            var propertiesCount = await _unitOfWork.PropertyRepo.GetAll()
                .CountAsync(p => p.UserId == userId && !p.IsDeleted);

            if (propertiesCount >= subscription.Package.MaxProperties)
                return new CanAddPropertyDto 
                { 
                    CanAdd = false, 
                    ErrorMessage = $"وصلت للحد الأقصى من الإعلانات ({subscription.Package.MaxProperties})", 
                    ErrorCode = "MAX_PROPERTIES_REACHED" 
                };

            // Check featured limit if adding featured property
            if (isFeatured)
            {
                var featuredCount = await _unitOfWork.PropertyRepo.GetAll()
                    .CountAsync(p => p.UserId == userId && !p.IsDeleted && p.IsFeatured);

                if (featuredCount >= subscription.Package.MaxFeatured)
                    return new CanAddPropertyDto 
                    { 
                        CanAdd = false, 
                        ErrorMessage = $"وصلت للحد الأقصى من الإعلانات المميزة ({subscription.Package.MaxFeatured})", 
                        ErrorCode = "MAX_FEATURED_REACHED" 
                    };
            }

            return new CanAddPropertyDto { CanAdd = true };
        }

        public async Task ProcessExpiringSubscriptionsAsync()
        {
            // Find subscriptions expiring in 3 days
            var expiringDate = DateTime.UtcNow.AddDays(3);
            var today = DateTime.UtcNow.Date;

            var expiringSubscriptions = await _unitOfWork.UserSubscriptionRepo.GetAll()
                .Include(s => s.Package)
                .Where(s => s.IsActive && !s.IsDeleted 
                    && s.EndDate.Date == expiringDate.Date)
                .ToListAsync();

            foreach (var sub in expiringSubscriptions)
            {
                var notification = new Notification
                {
                    UserId = sub.UserId,
                    Title = "⚠️ اشتراكك على وشك الانتهاء",
                    Message = $"اشتراكك في باقة {sub.Package.Name} سينتهي خلال 3 أيام. قم بتجديد الاشتراك للحفاظ على إعلاناتك.",
                    Type = NotificationType.SubscriptionExpiring,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.NotificationRepo.AddAsync(notification);
            }

            if (expiringSubscriptions.Any())
                await _unitOfWork.Save();
        }

        public async Task ProcessExpiredSubscriptionsAsync()
        {
            // Find expired subscriptions
            var expiredSubscriptions = await _unitOfWork.UserSubscriptionRepo.GetAll()
                .Include(s => s.Package)
                .Where(s => s.IsActive && !s.IsDeleted && s.EndDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var sub in expiredSubscriptions)
            {
                // Deactivate subscription
                sub.IsActive = false;
                sub.UpdatedAt = DateTime.UtcNow;

                // Hide user's properties
                var userProperties = await _unitOfWork.PropertyRepo.GetAll()
                    .Where(p => p.UserId == sub.UserId && !p.IsDeleted && p.Status == PropertyStatus.Active)
                    .ToListAsync();

                foreach (var prop in userProperties)
                {
                    prop.Status = PropertyStatus.Hidden;
                    prop.UpdatedAt = DateTime.UtcNow;
                }

                // Create notification
                var notification = new Notification
                {
                    UserId = sub.UserId,
                    Title = "❌ انتهى اشتراكك",
                    Message = $"انتهى اشتراكك في باقة {sub.Package.Name}. تم إخفاء إعلاناتك مؤقتاً. قم بتجديد الاشتراك لإظهارها مرة أخرى.",
                    Type = NotificationType.SubscriptionExpired,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.NotificationRepo.AddAsync(notification);
            }

            if (expiredSubscriptions.Any())
                await _unitOfWork.Save();
        }
    }
}
