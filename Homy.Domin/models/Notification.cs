using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;

        [MaxLength(1000)]
        public string? Message { get; set; }

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; } = false;

        // ربط اختياري بالإعلان
        public long? PropertyId { get; set; }
        public virtual Property? Property { get; set; }
    }

    public enum NotificationType : byte
    {
        PropertyApproved = 1,
        PropertyRejected = 2,
        PropertyChangesRequested = 3,
        VerificationApproved = 4,
        VerificationRejected = 5,
        SubscriptionCreated = 6,
        SubscriptionExpiring = 7,
        SubscriptionExpired = 8,
        PaymentFailed = 9,
        General = 10,
        PropertyLimitReached = 11
    }
}
