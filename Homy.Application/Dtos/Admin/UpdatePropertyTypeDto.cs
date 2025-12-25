using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Homy.Application.Dtos.Admin
{
    public class UpdatePropertyTypeDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم يجب ألا يتجاوز 100 حرف")]
        public string Name { get; set; }

        // Image upload
        public IFormFile? IconImage { get; set; }
        
        // Current icon URL for display/preview
        public string? CurrentIconUrl { get; set; }
    }
}
