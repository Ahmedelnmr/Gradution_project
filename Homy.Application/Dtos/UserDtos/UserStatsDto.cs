using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos.UserDtos
{
    public class UserStatsDto
    {
        public int TotalBuyers { get; set;  }
        public int TotalAgents { get; set; }
        public int PendingAgents { get; set; }
    }
}
