using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class PropertyImage : BaseEntity
    {
        public long PropertyId { get; set; }

        [Required, Url, MaxLength(1000)]
        public string ImageUrl { get; set; } = null!;

        public bool IsMain { get; set; } = false;
        public int SortOrder { get; set; } = 0;

        public virtual Property Property { get; set; } = null!;
    }
}
