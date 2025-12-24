using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos.Admin
{
    public class DashboardDto
    {
        public int UsersCount { get; set; }
        public int AdsCount { get; set; }
        public int ActivePropertiesCount { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int ProjectsCount { get; set; }
    }
}
