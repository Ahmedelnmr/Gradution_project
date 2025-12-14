using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos
{
    public class ProjectDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public string? CoverImageUrl { get; set; }

        public long CityId { get; set; }
        public string CityName { get; set; } = null!;

        public long? DistrictId { get; set; }
        public string? DistrictName { get; set; }

        public string? LocationDescription { get; set; }

        public bool IsActive { get; set; }

        // عدد الوحدات في المشروع
        public int PropertiesCount { get; set; }

        // السعر الأدنى للوحدات
        public decimal? MinPrice { get; set; }

        // نطاق المساحات
        public int? MinArea { get; set; }
        public int? MaxArea { get; set; }

        // سنة الإنشاء
        public int? Year { get; set; }

        // اسم المطور/الشركة
        public string? DeveloperName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
