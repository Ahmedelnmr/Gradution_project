using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class UserSubscription : BaseEntity
    {
        public Guid UserId { get; set; }
        public long PackageId { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountPaid { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Package Package { get; set; } = null!;
    }
}
