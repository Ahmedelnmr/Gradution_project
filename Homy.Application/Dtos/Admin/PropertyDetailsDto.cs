using Homy.Domin.models;
using System.Collections.Generic;

namespace Homy.Application.Dtos.Admin
{
    public class PropertyDetailsDto : PropertyListDto
    {
        public string? Description { get; set; }
        public string? DescriptionEn { get; set; } // Added
        public List<string> Images { get; set; } = new();
        public List<string> Amenities { get; set; } = new();
        
        // بيانات تواصل المعلن
        public string OwnerPhone { get; set; } = null!;
        public string? OwnerWhatsApp { get; set; }
        
        // الموقع
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? AddressDetails { get; set; }
        public string? AddressDetailsEn { get; set; } // Added
        
        // تفاصيل إضافية
        public FinishingType? FinishingType { get; set; }
        public byte? FloorNumber { get; set; }
        public bool IsFeatured { get; set; }
        public System.DateTime? FeaturedUntil { get; set; }
        
        // سجل المراجعات
        public List<PropertyReviewHistoryDto> ReviewHistory { get; set; } = new();
    }
}
