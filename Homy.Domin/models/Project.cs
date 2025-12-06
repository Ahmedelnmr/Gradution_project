using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class Project : BaseEntity
    {
        [Required, MaxLength(300)]
        public string Name { get; set; } = null!;            // ماونتن فيو ألموند – لافيستا رأس الحكمة

        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        [MaxLength(1000)]
        public string? CoverImageUrl { get; set; }

        public long CityId { get; set; }
        public virtual City City { get; set; } = null!;

        public long? DistrictId { get; set; }
        public virtual District? District { get; set; }

        [MaxLength(1000)]
        public string? LocationDescription { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
