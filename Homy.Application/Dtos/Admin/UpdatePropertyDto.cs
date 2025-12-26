using Microsoft.AspNetCore.Http;
using Homy.Domin.models;
using System.ComponentModel.DataAnnotations;

namespace Homy.Application.Dtos.Admin
{
    public class UpdatePropertyDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "عنوان الإعلان مطلوب")]
        [MaxLength(300)]
        public string Title { get; set; } = null!;
        public string? TitleEn { get; set; } // Added

        public string? Description { get; set; }
        public string? DescriptionEn { get; set; } // Added

        [Required(ErrorMessage = "نوع العقار مطلوب")]
        public long PropertyTypeId { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        public long CityId { get; set; }

        public long? DistrictId { get; set; }
        public long? ProjectId { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        public decimal Price { get; set; }

        public int? Area { get; set; }
        public byte? Rooms { get; set; }
        public byte? Bathrooms { get; set; }
        public byte? FloorNumber { get; set; }

        public FinishingType? FinishingType { get; set; }
        public PropertyPurpose Purpose { get; set; }

        public string? AddressDetails { get; set; }
        public string? AddressDetailsEn { get; set; } // Added
        
        public PropertyStatus Status { get; set; }

        // New Images to Add
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();
        public List<string> NewImageUrls { get; set; } = new List<string>();

        // IDs of images to delete
        public List<long> DeletedImageIds { get; set; } = new List<long>();
        
        // Main Image ID
        public long? MainImageId { get; set; }
        
        // List of existing images for display & management
        public List<ExistingImageDto> CurrentImages { get; set; } = new();
    }
}
