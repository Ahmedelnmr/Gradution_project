using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homy.Domin.models;

namespace Homy.Domin.Contract_Repo
{
    public interface IReports_Repo
    {
        // Users Statistics
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetUsersCountByMonthAsync(int year, int month);
        
        // Properties/Ads Statistics
        Task<int> GetTotalPropertiesCountAsync();
        Task<int> GetActivePropertiesCountAsync();
        Task<int> GetPropertiesCountByMonthAsync(int year, int month);
        Task<Dictionary<long, int>> GetPropertiesCountByPropertyTypeAsync();
        Task<Dictionary<long, int>> GetPropertiesCountByCityAsync();
        Task<int> GetTotalViewsCountAsync();
        
        // Subscriptions Statistics
        Task<int> GetSubscriptionsCountByMonthAsync(int year, int month);
        Task<decimal> GetRevenueByMonthAsync(int year, int month);
        Task<decimal> GetTotalRevenueByYearAsync(int year);
        Task<List<MonthlyRevenue>> GetMonthlyRevenuesAsync(int year);
        
        // Top Regions
        Task<List<CityStatistics>> GetTopCitiesAsync(int topCount = 5);
    }
    
    // Helper classes for complex queries
    public class MonthlyRevenue
    {
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        public int SubscriptionsCount { get; set; }
    }
    
    public class CityStatistics
    {
        public long CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public int PropertiesCount { get; set; }
        public int ViewsCount { get; set; }
    }
}