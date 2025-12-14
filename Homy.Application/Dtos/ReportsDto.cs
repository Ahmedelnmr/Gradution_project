using System;
using System.Collections.Generic;

namespace Homy.Application.Dtos
{
    public class ReportsDto
    {
        public int TotalUsers { get; set; }
        public int TotalAds { get; set; }
        public int ActiveAds { get; set; }
        public decimal TotalRevenue { get; set; }
        public int MonthlyViews { get; set; }
        
        // نسب النمو
        public decimal UsersGrowthPercentage { get; set; }
        public decimal AdsGrowthPercentage { get; set; }
        public decimal ViewsGrowthPercentage { get; set; }
        public decimal RevenueGrowthPercentage { get; set; }
        
        // الإيرادات الشهرية
        public List<MonthlyRevenueDto> MonthlyRevenues { get; set; } = new List<MonthlyRevenueDto>();
        
        // توزيع الإعلانات حسب النوع
        public List<AdsDistributionDto> AdsDistribution { get; set; } = new List<AdsDistributionDto>();
        
        // أكثر المناطق نشاطاً
        public List<TopRegionDto> TopRegions { get; set; } = new List<TopRegionDto>();
        
        // مقارنة الأداء الشهري
        public MonthlyComparisonDto MonthlyComparison { get; set; } = new MonthlyComparisonDto();
    }
}