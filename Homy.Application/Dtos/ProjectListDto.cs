using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos
{
    public class ProjectListDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? NameEn { get; set; } // Added
        public string? LogoUrl { get; set; }
        public string CityName { get; set; } = null!;
        public string? DistrictName { get; set; }
        public bool IsActive { get; set; }
        public string? LocationDescription { get; set; }
        public string? LocationDescriptionEn { get; set; } // Added
        public int PropertiesCount { get; set; }
        public decimal? MinPrice { get; set; }

        // Computed Property
        public ProjectTypeEnum ComputedType => DetermineProjectType();
        public string TypeText => GetProjectTypeText(ComputedType);

        private ProjectTypeEnum DetermineProjectType()
        {
            var nameLower = Name?.ToLower() ?? "";
            var locationLower = LocationDescription?.ToLower() ?? "";

            if (PropertiesCount == 0 ||
                nameLower.Contains("قيد") ||
                nameLower.Contains("إنشاء") ||
                nameLower.Contains("تحت التطوير") ||
                locationLower.Contains("قيد الإنشاء"))
            {
                return ProjectTypeEnum.UnderConstruction;
            }

            if (nameLower.Contains("مول") ||
                nameLower.Contains("تجاري") ||
                nameLower.Contains("أعمال") ||
                nameLower.Contains("محلات") ||
                nameLower.Contains("مكاتب") ||
                locationLower.Contains("تجاري"))
            {
                return ProjectTypeEnum.CommercialProject;
            }

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
    public enum ProjectTypeEnum : byte
    {
        ResidentialCompound = 1,
        CommercialProject = 2,
        UnderConstruction = 3
    }
}
