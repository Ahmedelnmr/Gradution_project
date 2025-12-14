using Homy.Domin.models;
using System.ComponentModel.DataAnnotations;

namespace Homy.Application.Dtos.Admin
{
    public class CreatePropertyReviewDto
    {
        [Required]
        public long PropertyId { get; set; }
        
        [Required]
        public ReviewAction Action { get; set; }
        
        [MaxLength(1000)]
        public string? Message { get; set; }
    }
}
