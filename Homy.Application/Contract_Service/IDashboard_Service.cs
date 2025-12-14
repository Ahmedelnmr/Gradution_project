using Homy.Application.Dtos.Admin;
using System.Threading.Tasks;

namespace Homy.Domin.Contract_Service
{
    public interface IDashboard_Service
    {
        Task<DashboardDto> GetDashboardDataAsync();
    }
}
