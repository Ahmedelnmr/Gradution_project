using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class PropertyReview : BaseEntity
    {
        public long PropertyId { get; set; }
        public virtual Property Property { get; set; } = null!;

        public Guid AdminId { get; set; }
        public virtual User Admin { get; set; } = null!;

        public ReviewAction Action { get; set; }

        [MaxLength(1000)]
        public string? Message { get; set; }  // رسالة الأدمن للمعلن
    }

    public enum ReviewAction : byte
    {
        Approved = 1,           // تم القبول
        Rejected = 2,           // مرفوض
        ChangesRequested = 3    // محتاج تعديلات
    }
}
