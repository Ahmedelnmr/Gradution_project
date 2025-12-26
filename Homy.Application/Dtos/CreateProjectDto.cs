using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos
{
    public class CreateProjectDto
    {
        [Required(ErrorMessage = "اسم المشروع مطلوب")]
        [MaxLength(300)]
        public string Name { get; set; } = null!;

        [MaxLength(300)]
        public string? NameEn { get; set; } // Added

        // Image uploads
        public IFormFile? LogoImage { get; set; }
        public IFormFile? CoverImage { get; set; }
        
        // Store paths after upload
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        public long CityId { get; set; }

        public long? DistrictId { get; set; }

        [MaxLength(1000)]
        public string? LocationDescription { get; set; }

        [MaxLength(1000)]
        public string? LocationDescriptionEn { get; set; } // Added

        public bool IsActive { get; set; } = true;
    }
}
