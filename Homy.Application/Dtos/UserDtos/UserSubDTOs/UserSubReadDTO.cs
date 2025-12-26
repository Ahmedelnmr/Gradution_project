using Homy.Domin.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos.UserDtos.UserSubDTOs
{
    public class UserSubReadDTO
    {
        public Guid UserId { get; set; }
        public long PackageId { get; set; }
        public string? PackageName { get; set; } // Added for display
        public string? UserName { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountPaid { get; set; }

    }
}
