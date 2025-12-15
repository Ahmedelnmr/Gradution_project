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

        // Computed Property - لا يحتاج تعديل في Model
        public ProjectTypeEnum ComputedType => DetermineProjectType();
        public string TypeText => GetProjectTypeText(ComputedType);

        // إحصائيات
        public int PropertiesCount { get; set; }
        public decimal? MinPrice { get; set; }
        public int? MinArea { get; set; }
        public int? MaxArea { get; set; }
        public int? Year { get; set; }
        public string? DeveloperName { get; set; }

        public DateTime CreatedAt { get; set; }

        // منطق تحديد النوع بناءً على البيانات الموجودة
        private ProjectTypeEnum DetermineProjectType()
        {
            var nameLower = Name?.ToLower() ?? "";
            var locationLower = LocationDescription?.ToLower() ?? "";

            // قيد الإنشاء: إذا كان عدد الوحدات = 0 أو يحتوي على كلمات دلالية
            if (PropertiesCount == 0 ||
                nameLower.Contains("قيد") ||
                nameLower.Contains("إنشاء") ||
                nameLower.Contains("تحت التطوير") ||
                locationLower.Contains("قيد الإنشاء"))
            {
                return ProjectTypeEnum.UnderConstruction;
            }

            // مشروع تجاري: إذا يحتوي على كلمات دلالية
            if (nameLower.Contains("مول") ||
                nameLower.Contains("تجاري") ||
                nameLower.Contains("أعمال") ||
                nameLower.Contains("محلات") ||
                nameLower.Contains("مكاتب") ||
                locationLower.Contains("تجاري"))
            {
                return ProjectTypeEnum.CommercialProject;
            }

            // كمبوند سكني: الافتراضي
            return ProjectTypeEnum.ResidentialCompound;
        }

        private string GetProjectTypeText(ProjectTypeEnum type)
        {
            return type switch
            {
                ProjectTypeEnum.ResidentialCompound => "كمبوند سكني",
                ProjectTypeEnum.CommercialProject => "مشروع تجاري",
                ProjectTypeEnum.UnderConstruction => "قيد الإنشاء",
                _ => "غير محدد"
            };
        }
    }
}
