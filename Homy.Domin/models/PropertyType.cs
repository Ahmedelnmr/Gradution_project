using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class PropertyType : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;        // شقة – فيلا – أرض زراعية – محل

        [MaxLength(500)]
        public string? IconUrl { get; set; }

        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
