using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos.Admin
{
    public class DashboardDto
    {
        // Users Statistics
        public int UsersCount { get; set; }
        public int ActiveUsersCount { get; set; }
        public int VerifiedAgentsCount { get; set; }
        public int UnverifiedAgentsCount { get; set; }

        // Properties Statistics
        public int AdsCount { get; set; }
        public int ActivePropertiesCount { get; set; }
        public int PendingPropertiesCount { get; set; }
        public int RejectedPropertiesCount { get; set; }
        public int FeaturedPropertiesCount { get; set; }

        // Subscriptions Statistics
        public int ActiveSubscriptionsCount { get; set; }
        public int TotalSubscriptionsCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }

        // Additional Statistics
        public int TotalPropertyTypes { get; set; }
        public int TotalProjects { get; set; }

        // Property Types Distribution for Charts (Key: PropertyType Name, Value: Count)
        public Dictionary<string, int> PropertyTypesDistribution { get; set; } = new Dictionary<string, int>();

        // Additional User Statistics
        public int OwnersCount { get; set; }
        public int AgentsCount { get; set; }

        // Sold/Rented Properties
        public int SoldOrRentedPropertiesCount { get; set; }
    }
}
