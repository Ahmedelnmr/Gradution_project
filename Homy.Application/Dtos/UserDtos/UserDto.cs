using Homy.Domin.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos.UserDtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string WhatsAppNumber { get; set; }
        public string Email { get; set; }
        public string ProfileImageUrl { get; set; }
        public UserRole Role { get; set; }
        public string RoleText { get; set; } 
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public int PropertiesCount { get; set; }
        public int SavedPropertiesCount { get; set; }
        public bool HasActiveSubscription { get; set; }

        // Verification Documents
        public string? IdCardFrontUrl { get; set; }
        public string? IdCardBackUrl { get; set; }
        public string? SelfieWithIdUrl { get; set; }
        public string? VerificationRejectReason { get; set; }
    }
}
