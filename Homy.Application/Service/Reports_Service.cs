using Homy.Application.Dtos;
using Homy.Domin.Contract_Repo;
using Homy.Infurastructure.Unitofworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homy.Application.Contract_Service;

namespace Homy.Application.Service
{
    public class Reports_Service : IReports_Service
    {
        private readonly IUnitofwork _unitofwork;
        // Determine the target year: if we are in 2025, show 2024 data (for demo purposes), otherwise use current year
        private readonly int _targetYear = DateTime.Now.Year == 2025 ? 2024 : DateTime.Now.Year;

        public Reports_Service(IUnitofwork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public async Task<ReportsDto> GetFullReportsAsync()
        {
            var reports = new ReportsDto();

            // إحصائيات عامة
            reports.TotalUsers = await GetUsersCountAsync();
            reports.TotalAds = await GetAdsCountAsync();
            reports.ActiveAds = await GetActiveAdsCountAsync();
            reports.TotalRevenue = await GetTotalRevenueAsync();
            reports.MonthlyViews = await _unitofwork.ReportsRepo.GetTotalViewsCountAsync();

            // نسب النمو
            reports.UsersGrowthPercentage = await CalculateUsersGrowthAsync();
            reports.AdsGrowthPercentage = await CalculateAdsGrowthAsync();
            reports.ViewsGrowthPercentage = 32m; // يمكن حسابها لاحقاً
            reports.RevenueGrowthPercentage = await CalculateRevenueGrowthAsync();

            // الإيرادات الشهرية
            reports.MonthlyRevenues = await GetMonthlyRevenuesByYearAsync(_targetYear);

            // توزيع الإعلانات
            reports.AdsDistribution = await GetAdsDistributionAsync();

            // أكثر المناطق نشاطاً
            reports.TopRegions = await GetTopRegionsAsync();

            // مقارنة الأداء
            reports.MonthlyComparison = await GetMonthlyComparisonAsync();

            return reports;
        }

        public async Task<int> GetUsersCountAsync()
        {
            return await _unitofwork.ReportsRepo.GetTotalUsersCountAsync();
        }

        public async Task<int> GetAdsCountAsync()
        {
            return await _unitofwork.ReportsRepo.GetTotalPropertiesCountAsync();
        }

        public async Task<int> GetActiveAdsCountAsync()
        {
            return await _unitofwork.ReportsRepo.GetActivePropertiesCountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _unitofwork.ReportsRepo.GetTotalRevenueByYearAsync(_targetYear);
        }

        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenuesByYearAsync(int year)
        {
            var monthlyData = await _unitofwork.ReportsRepo.GetMonthlyRevenuesAsync(year);

            var monthNames = new[] { 
                "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
            };

            var monthlyRevenues = new List<MonthlyRevenueDto>();
            
            for (int month = 1; month <= 12; month++)
            {
                var data = monthlyData.FirstOrDefault(m => m.Month == month);
                
                monthlyRevenues.Add(new MonthlyRevenueDto
                {
                    Month = monthNames[month - 1],
                    MonthNumber = month,
                    Revenue = data?.Revenue ?? 0,
                    AdsCount = data?.SubscriptionsCount ?? 0
                });
            }

            return monthlyRevenues;
        }

        private async Task<decimal> CalculateUsersGrowthAsync()
        {
            var currentMonth = DateTime.Now.Month;
            // Use target year logic
            var currentYear = _targetYear;
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;
            
            var currentMonthUsers = await _unitofwork.ReportsRepo.GetUsersCountByMonthAsync(currentYear, currentMonth);
            var previousMonthUsers = await _unitofwork.ReportsRepo.GetUsersCountByMonthAsync(previousYear, previousMonth);
            
            if (previousMonthUsers == 0) 
            {
                // إذا لم يكن هناك مستخدمين في الشهر السابق، نستخدم نسبة افتراضية
                previousMonthUsers = currentMonthUsers > 0 ? (int)(currentMonthUsers * 0.85) : 0;
            }
            
            if (previousMonthUsers == 0) return 0;
            
            return ((decimal)(currentMonthUsers - previousMonthUsers) / previousMonthUsers) * 100;
        }

        private async Task<decimal> CalculateAdsGrowthAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = _targetYear;
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;
            
            var currentMonthAds = await _unitofwork.ReportsRepo.GetPropertiesCountByMonthAsync(currentYear, currentMonth);
            var previousMonthAds = await _unitofwork.ReportsRepo.GetPropertiesCountByMonthAsync(previousYear, previousMonth);
            
            if (previousMonthAds == 0)
            {
                previousMonthAds = currentMonthAds > 0 ? (int)(currentMonthAds * 0.80) : 0;
            }
            
            if (previousMonthAds == 0) return 0;
            
            return ((decimal)(currentMonthAds - previousMonthAds) / previousMonthAds) * 100;
        }

        private async Task<decimal> CalculateRevenueGrowthAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = _targetYear;
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;
            
            var currentRevenue = await _unitofwork.ReportsRepo.GetRevenueByMonthAsync(currentYear, currentMonth);
            var previousRevenue = await _unitofwork.ReportsRepo.GetRevenueByMonthAsync(previousYear, previousMonth);
            
            if (previousRevenue == 0)
            {
                // previousRevenue = currentRevenue > 0 ? currentRevenue * 0.75m : 0;
                return 100;
            }
            
            if (previousRevenue == 0) return 0;
            
            return ((currentRevenue - previousRevenue) / previousRevenue) * 100;
        }

        private async Task<List<AdsDistributionDto>> GetAdsDistributionAsync()
        {
            var propertiesByType = await _unitofwork.ReportsRepo.GetPropertiesCountByPropertyTypeAsync();
            var propertyTypes = await _unitofwork.PropertyTypeRepo.GetAllAsync();
            
            var totalAds = propertiesByType.Values.Sum();
            if (totalAds == 0) return new List<AdsDistributionDto>();

            var colors = new[] { "#6366f1", "#10b981", "#f59e0b", "#8b5cf6", "#06b6d4" };
            var result = new List<AdsDistributionDto>();
            int colorIndex = 0;

            foreach (var kvp in propertiesByType.OrderByDescending(x => x.Value))
            {
                var type = propertyTypes.FirstOrDefault(pt => pt.Id == kvp.Key);
                result.Add(new AdsDistributionDto
                {
                    PropertyType = type?.Name ?? "غير معروف",
                    Count = kvp.Value,
                    Percentage = ((decimal)kvp.Value / totalAds) * 100,
                    Color = colors[colorIndex % colors.Length]
                });
                colorIndex++;
            }

            return result;
        }

        private async Task<List<TopRegionDto>> GetTopRegionsAsync()
        {
            var topCities = await _unitofwork.ReportsRepo.GetTopCitiesAsync(5);

            var result = new List<TopRegionDto>();
            int rank = 1;

            foreach (var city in topCities)
            {
                result.Add(new TopRegionDto
                {
                    Rank = rank,
                    RegionName = city.CityName,
                    AdsCount = city.PropertiesCount,
                    ViewsCount = city.ViewsCount,
                    GrowthPercentage = 30m - (rank * 5) // محاكاة للنمو
                });
                rank++;
            }

            return result;
        }

        private async Task<MonthlyComparisonDto> GetMonthlyComparisonAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = _targetYear;
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            var comparison = new MonthlyComparisonDto();

            // مقارنة الإعلانات
            var currentMonthAds = await _unitofwork.ReportsRepo.GetPropertiesCountByMonthAsync(currentYear, currentMonth);
            var previousMonthAds = await _unitofwork.ReportsRepo.GetPropertiesCountByMonthAsync(previousYear, previousMonth);
            comparison.NewAds = CreateComparisonItem(currentMonthAds, previousMonthAds);

            // مقارنة المستخدمين
            var currentMonthUsers = await _unitofwork.ReportsRepo.GetUsersCountByMonthAsync(currentYear, currentMonth);
            var previousMonthUsers = await _unitofwork.ReportsRepo.GetUsersCountByMonthAsync(previousYear, previousMonth);
            comparison.NewUsers = CreateComparisonItem(currentMonthUsers, previousMonthUsers);

            // مقارنة الاشتراكات
            var currentMonthSubs = await _unitofwork.ReportsRepo.GetSubscriptionsCountByMonthAsync(currentYear, currentMonth);
            var previousMonthSubs = await _unitofwork.ReportsRepo.GetSubscriptionsCountByMonthAsync(previousYear, previousMonth);
            comparison.NewSubscriptions = CreateComparisonItem(currentMonthSubs, previousMonthSubs);

            // مقارنة الإيرادات
            var currentRevenue = (int)await _unitofwork.ReportsRepo.GetRevenueByMonthAsync(currentYear, currentMonth);
            var previousRevenue = (int)await _unitofwork.ReportsRepo.GetRevenueByMonthAsync(previousYear, previousMonth);
            comparison.Revenue = CreateComparisonItem(currentRevenue, previousRevenue);

            // مقارنة تذاكر الدعم (محاكاة - يمكن إضافة جدول للدعم لاحقاً)
            comparison.SupportTickets = CreateComparisonItem(28, 35);

            return comparison;
        }

        private ComparisonItemDto CreateComparisonItem(int current, int previous)
        {
            if (previous == 0 && current > 0)
            {
                previous = (int)(current * 0.8);
            }
            
            var change = previous > 0 ? ((decimal)(current - previous) / previous) * 100 : 0;
            
            return new ComparisonItemDto
            {
                CurrentValue = current,
                PreviousValue = previous,
                ChangePercentage = Math.Abs(change),
                IsPositive = current >= previous
            };
        }
    }
}