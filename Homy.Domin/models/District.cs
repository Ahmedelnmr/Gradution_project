using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class District : BaseEntity
    {
        [Required]
        public long CityId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;        // الكوثر – جرجا – الزمالك

        public virtual City City { get; set; } = null!;
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
