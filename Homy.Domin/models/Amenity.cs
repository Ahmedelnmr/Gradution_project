using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class Amenity : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;             // أسانسير – بلكونة – جراج

        [MaxLength(500)]
        public string? IconUrl { get; set; }

        public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
    }
}
