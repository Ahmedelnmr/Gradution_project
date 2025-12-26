using System;

namespace Homy.Application.Dtos.Admin
{
    public class PropertyTypeDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? NameEn { get; set; } // Added
        public string? IconUrl { get; set; }
        public int PropertiesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
