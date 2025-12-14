using System.ComponentModel.DataAnnotations;

namespace Homy.Application.Dtos.Admin
{
    public class CreatePropertyTypeDto
    {
        [Required(ErrorMessage = "اسم النوع مطلوب")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        
        [MaxLength(500)]
        public string? IconUrl { get; set; }
    }
}
