using Homy.Application.Dtos;
using System.Threading.Tasks;

namespace Homy.Application.Contract_Service
{
    public interface IReports_Service
    {
        Task<ReportsDto> GetFullReportsAsync();
        Task<int> GetUsersCountAsync();
        Task<int> GetAdsCountAsync();
        Task<int> GetActiveAdsCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenuesByYearAsync(int year);
    }
}