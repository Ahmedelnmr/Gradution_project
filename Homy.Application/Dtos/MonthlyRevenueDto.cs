using System;
using System.Collections.Generic;

namespace Homy.Application.Dtos
{
 public class MonthlyRevenueDto
    {
        public string Month { get; set; } = string.Empty;
        public int MonthNumber { get; set; }
        public decimal Revenue { get; set; }
        public int AdsCount { get; set; }
    }
}