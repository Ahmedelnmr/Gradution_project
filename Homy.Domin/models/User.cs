using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class User : IdentityUser<Guid>
    {
        [Required, MaxLength(200)]
        public string FullName { get; set; } = null!;




        [Phone, MaxLength(20)]
        public string? WhatsAppNumber { get; set; }

        public bool IsDeleted { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public UserRole Role { get; set; } = UserRole.Owner; // Default to Owner




        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; }

        // العلاقات
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
        public virtual ICollection<SavedProperty> SavedProperties { get; set; } = new List<SavedProperty>();
        public virtual ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    public enum UserRole : byte { Owner = 1, Agent = 2, Admin = 3 }
}

