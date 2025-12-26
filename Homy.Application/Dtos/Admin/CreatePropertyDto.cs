using Microsoft.AspNetCore.Http;
using Homy.Domin.models;
using System.ComponentModel.DataAnnotations;

namespace Homy.Application.Dtos.Admin
{
    public class CreatePropertyDto
    {
        [Required(ErrorMessage = "عنوان الإعلان مطلوب")]
        [MaxLength(300)]
        public string Title { get; set; } = null!;

        [MaxLength(300)]
        public string? TitleEn { get; set; } // Added

        public string? Description { get; set; }
        public string? DescriptionEn { get; set; } // Added

        [Required(ErrorMessage = "نوع العقار مطلوب")]
        public long PropertyTypeId { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        public long CityId { get; set; }

        public long? DistrictId { get; set; }

        // Optional Project association
        public long? ProjectId { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "السعر يجب أن يكون قيمة موجبة")]
        public decimal Price { get; set; }

        public int? Area { get; set; }
        public byte? Rooms { get; set; }
        public byte? Bathrooms { get; set; }
        public byte? FloorNumber { get; set; }

        public FinishingType? FinishingType { get; set; }
        public PropertyPurpose Purpose { get; set; } = PropertyPurpose.ForSale;

        public string? AddressDetails { get; set; }
        public string? AddressDetailsEn { get; set; } // Added

        // Images
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        
        // Populated by Controller
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
