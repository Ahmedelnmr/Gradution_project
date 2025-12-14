using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos
{
    public class UpdateProjectDto
    {
        [Required]
        public long Id { get; set; }

        [Required(ErrorMessage = "اسم المشروع مطلوب")]
        [MaxLength(300)]
        public string Name { get; set; } = null!;

        [Url]
        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        [Url]
        [MaxLength(1000)]
        public string? CoverImageUrl { get; set; }

        [Required]
        public long CityId { get; set; }

        public long? DistrictId { get; set; }

        [MaxLength(1000)]
        public string? LocationDescription { get; set; }

        public bool IsActive { get; set; }
    }
}
