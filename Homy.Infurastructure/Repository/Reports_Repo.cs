using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Infurastructure.Repository
{
    public class Reports_Repo : IReports_Repo
    {
        private readonly HomyContext _context;

        public Reports_Repo(HomyContext context)
        {
            _context = context;
        }

        // ==================== Users Statistics ====================
        
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.Users.CountAsync(u => !u.IsDeleted);
        }

        public async Task<int> GetUsersCountByMonthAsync(int year, int month)
        {
            return await _context.Users
                .CountAsync(u => !u.IsDeleted && 
                                 u.LockoutEnd == null &&
                                 u.Id != Guid.Empty);
        }

        // ==================== Properties Statistics ====================
        
        public async Task<int> GetTotalPropertiesCountAsync()
        {
            return await _context.Properties.CountAsync();
        }

        public async Task<int> GetActivePropertiesCountAsync()
        {
            return await _context.Properties
                .CountAsync(p => p.Status == PropertyStatus.Active);
        }

        public async Task<int> GetPropertiesCountByMonthAsync(int year, int month)
        {
            return await _context.Properties
                .CountAsync(p => p.CreatedAt.Year == year && 
                                p.CreatedAt.Month == month);
        }

        public async Task<Dictionary<long, int>> GetPropertiesCountByPropertyTypeAsync()
        {
            return await _context.Properties
                .GroupBy(p => p.PropertyTypeId)
                .Select(g => new { TypeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TypeId, x => x.Count);
        }

        public async Task<Dictionary<long, int>> GetPropertiesCountByCityAsync()
        {
            return await _context.Properties
                .GroupBy(p => p.CityId)
                .Select(g => new { CityId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CityId, x => x.Count);
        }

        public async Task<int> GetTotalViewsCountAsync()
        {
            return await _context.Properties.SumAsync(p => p.ViewCount);
        }

        // ==================== Subscriptions Statistics ====================
        
        public async Task<int> GetSubscriptionsCountByMonthAsync(int year, int month)
        {
            return await _context.UserSubscriptions
                .CountAsync(s => s.StartDate.Year == year && 
                                s.StartDate.Month == month);
        }

        public async Task<decimal> GetRevenueByMonthAsync(int year, int month)
        {
            return await _context.UserSubscriptions
                .Where(s => s.StartDate.Year == year && 
                           s.StartDate.Month == month &&
                           s.IsActive)
                .SumAsync(s => s.AmountPaid);
        }

        public async Task<decimal> GetTotalRevenueByYearAsync(int year)
        {
            return await _context.UserSubscriptions
                .Where(s => s.StartDate.Year == year && s.IsActive)
                .SumAsync(s => s.AmountPaid);
        }

        public async Task<List<MonthlyRevenue>> GetMonthlyRevenuesAsync(int year)
        {
            return await _context.UserSubscriptions
                .Where(s => s.StartDate.Year == year && s.IsActive)
                .GroupBy(s => s.StartDate.Month)
                .Select(g => new MonthlyRevenue
                {
                    Month = g.Key,
                    Revenue = g.Sum(s => s.AmountPaid),
                    SubscriptionsCount = g.Count()
                })
                .OrderBy(m => m.Month)
                .ToListAsync();
        }

        // ==================== Top Regions ====================
        
        public async Task<List<CityStatistics>> GetTopCitiesAsync(int topCount = 5)
        {
            var citiesStats = await _context.Properties
                .GroupBy(p => new { p.CityId, p.City.Name })
                .Select(g => new CityStatistics
                {
                    CityId = g.Key.CityId,
                    CityName = g.Key.Name,
                    PropertiesCount = g.Count(),
                    ViewsCount = g.Sum(p => p.ViewCount)
                })
                .OrderByDescending(c => c.PropertiesCount)
                .Take(topCount)
                .ToListAsync();

            return citiesStats;
        }
    }
}