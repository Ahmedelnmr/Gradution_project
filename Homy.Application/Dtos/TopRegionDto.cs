 using System;
using System.Collections.Generic;

namespace Homy.Application.Dtos
{
 public class TopRegionDto
    {
        public int Rank { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public int AdsCount { get; set; }
        public int ViewsCount { get; set; }
        public decimal GrowthPercentage { get; set; }
    }
}