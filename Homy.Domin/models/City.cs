using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class City : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;        // سوهاج – القاهرة – الإسكندرية

        public virtual ICollection<District> Districts { get; set; } = new List<District>();
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
