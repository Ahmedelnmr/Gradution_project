using Homy.Domin.models;
using System;

namespace Homy.Application.Dtos.Admin
{
    public class PropertyListDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? RentPriceMonthly { get; set; }
        public string PropertyTypeName { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string? DistrictName { get; set; }
        public byte? Rooms { get; set; }
        public byte? Bathrooms { get; set; }
        public int? Area { get; set; }
        
        // بيانات المعلن
        public Guid UserId { get; set; }
        public string OwnerName { get; set; } = null!;
        public string? OwnerImage { get; set; }
        
        public PropertyStatus Status { get; set; }
        public PropertyPurpose Purpose { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
