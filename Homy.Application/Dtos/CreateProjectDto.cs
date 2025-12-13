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

        [Url(ErrorMessage = "رابط الشعار غير صحيح")]
        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        [Url(ErrorMessage = "رابط الصورة غير صحيح")]
        [MaxLength(1000)]
        public string? CoverImageUrl { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        public long CityId { get; set; }

        public long? DistrictId { get; set; }

        [MaxLength(1000)]
        public string? LocationDescription { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
