using System;
using System.Collections.Generic;

namespace Homy.Application.Dtos
{ 
 public class AdsDistributionDto
    {
        public string PropertyType { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}