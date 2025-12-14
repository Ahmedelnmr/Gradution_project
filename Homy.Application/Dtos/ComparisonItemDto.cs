using System;
using System.Collections.Generic;

namespace Homy.Application.Dtos
{
public class ComparisonItemDto
    {
        public int CurrentValue { get; set; }
        public int PreviousValue { get; set; }
        public decimal ChangePercentage { get; set; }
        public bool IsPositive { get; set; }
    }
}