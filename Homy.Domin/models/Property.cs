using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.models
{
    public class Property : BaseEntity
    {
        [Required, MaxLength(500)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public long PropertyTypeId { get; set; }
        public virtual PropertyType PropertyType { get; set; } = null!;

        public long CityId { get; set; }
        public virtual City City { get; set; } = null!;

        public long? DistrictId { get; set; }
        public virtual District? District { get; set; }

        public long? ProjectId { get; set; }                     // ← جديد
        public virtual Project? Project { get; set; }          // ← جديد

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RentPriceMonthly { get; set; }

        public int? Area { get; set; }
        public byte? Rooms { get; set; }
        public byte? Bathrooms { get; set; }
        public byte? FloorNumber { get; set; }

        public FinishingType? FinishingType { get; set; }
        public PropertyPurpose Purpose { get; set; } = PropertyPurpose.ForSale;

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        [MaxLength(1000)]
        public string? AddressDetails { get; set; }

        public bool IsAgricultural { get; set; } = false;

        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int ViewCount { get; set; } = 0;
        public int WhatsAppClicks { get; set; } = 0;
        public int PhoneClicks { get; set; } = 0;

        public bool IsFeatured { get; set; } = false;
        public DateTime? FeaturedUntil { get; set; }

        public PropertyStatus Status { get; set; } = PropertyStatus.Active;

        // العلاقات
        public virtual ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
        public virtual ICollection<SavedProperty> SavedByUsers { get; set; } = new List<SavedProperty>();
    }

    public enum FinishingType : byte { None = 0, Semi = 1, Full = 2 }
    public enum PropertyPurpose : byte { ForSale = 1, ForRent = 2, Both = 3 }
    public enum PropertyStatus : byte { Active = 1, SoldOrRented = 2, Hidden = 3 }
}

