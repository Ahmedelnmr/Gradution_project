using Homy.Domin.models;
using System;

namespace Homy.Application.Dtos.Admin
{
    public class PropertyFilterDto
    {
        // الفلاتر
        public PropertyStatus? Status { get; set; }
        public PropertyPurpose? Purpose { get; set; }
        public long? PropertyTypeId { get; set; }
        public long? CityId { get; set; }
        public Guid? UserId { get; set; }
        
        // البحث (يبحث في: العنوان، الوصف، اسم المعلن)
        public string? Search { get; set; }
        
        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
