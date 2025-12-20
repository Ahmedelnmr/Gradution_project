using System.ComponentModel.DataAnnotations;

namespace Homy.Application.Dtos
{
    public class PackageDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 3650, ErrorMessage = "Duration must be at least 1 day")]
        [Display(Name = "Duration (Days)")]
        public int DurationDays { get; set; } = 30;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Max Properties must be at least 1")]
        [Display(Name = "Max Properties")]
        public int MaxProperties { get; set; } = 10;

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Max Featured Properties")]
        public int MaxFeatured { get; set; } = 3;

        [Display(Name = "Can Bump Up?")]
        public bool CanBumpUp { get; set; } = false;
        
        // Stats for view
        public int SubscriptionsCount { get; set; }
    }
}
