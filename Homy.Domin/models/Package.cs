using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class Package : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;           // برونز – فضي – ذهبي

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public int DurationDays { get; set; } = 30;
        public int MaxProperties { get; set; } = 10;
        public int MaxFeatured { get; set; } = 3;
        public bool CanBumpUp { get; set; } = false;

        public virtual ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
    }
}
