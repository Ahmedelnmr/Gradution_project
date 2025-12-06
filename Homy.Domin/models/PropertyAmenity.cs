using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class PropertyAmenity : BaseEntity
    {
        public long PropertyId { get; set; }
        public long AmenityId { get; set; }

        public virtual Property Property { get; set; } = null!;
        public virtual Amenity Amenity { get; set; } = null!;
    }
}
