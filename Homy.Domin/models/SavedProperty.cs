using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class SavedProperty : BaseEntity
    {
        public Guid UserId { get; set; }
        public long PropertyId { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.Now;

        public virtual User User { get; set; } = null!;
        public virtual Property Property { get; set; } = null!;
    }
}
