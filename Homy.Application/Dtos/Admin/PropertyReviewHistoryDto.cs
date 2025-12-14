using Homy.Domin.models;
using System;

namespace Homy.Application.Dtos.Admin
{
    public class PropertyReviewHistoryDto
    {
        public string AdminName { get; set; } = null!;
        public ReviewAction Action { get; set; }
        public string? Message { get; set; }
        public DateTime ReviewedAt { get; set; }
    }
}
