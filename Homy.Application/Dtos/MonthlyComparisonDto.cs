using System;
using System.Collections.Generic;

namespace Homy.Application.Dtos
{ 
 public class MonthlyComparisonDto
    {
        public ComparisonItemDto NewAds { get; set; } = new ComparisonItemDto();
        public ComparisonItemDto NewUsers { get; set; } = new ComparisonItemDto();
        public ComparisonItemDto NewSubscriptions { get; set; } = new ComparisonItemDto();
        public ComparisonItemDto Revenue { get; set; } = new ComparisonItemDto();
        public ComparisonItemDto SupportTickets { get; set; } = new ComparisonItemDto();
    }
}