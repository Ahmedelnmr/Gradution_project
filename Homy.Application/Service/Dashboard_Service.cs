 using Homy.Application.Dtos.Admin;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Homy.Infurastructure.Service
{
    public class Dashboard_Service : IDashboard_Service
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public Dashboard_Service(IUnitofwork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            // Users Statistics - Load to memory first to avoid LINQ translation issues with enum
            var allUsers = await _userManager.Users.ToListAsync();
            var usersCount = allUsers.Count;
            var activeUsersCount = allUsers.Count(u => u.IsActive);
            var verifiedAgentsCount = allUsers.Count(u => u.Role == UserRole.Agent && u.IsVerified);
            var unverifiedAgentsCount = allUsers.Count(u => u.Role == UserRole.Agent && !u.IsVerified);
            
            // Separate Owners and Agents
            var ownersCount = allUsers.Count(u => u.Role == UserRole.Owner);
            var agentsCount = allUsers.Count(u => u.Role == UserRole.Agent);

            // Properties Statistics  
            var allProperties = await _unitOfWork.PropertyRepo.GetAll()
                .Where(p => !p.IsDeleted)
                .ToListAsync();
            
            var adsCount = allProperties.Count;
            var activePropertiesCount = allProperties.Count(p => p.Status == PropertyStatus.Active);
            var pendingPropertiesCount = allProperties.Count(p => p.Status == PropertyStatus.PendingReview);
            var rejectedPropertiesCount = allProperties.Count(p => p.Status == PropertyStatus.Rejected);
            var featuredPropertiesCount = allProperties.Count(p => p.IsFeatured);
            var soldOrRentedPropertiesCount = allProperties.Count(p => p.Status == PropertyStatus.SoldOrRented);

            // Subscriptions Statistics
            var allSubscriptions = await _unitOfWork.UserSubscriptionRepo.GetAll()
                .Include(s => s.Package)
                .Where(s => !s.IsDeleted)
                .ToListAsync();

            var activeSubscriptionsCount = allSubscriptions.Count(s => s.IsActive && s.EndDate > DateTime.UtcNow);
            var totalSubscriptionsCount = allSubscriptions.Count;
            var totalRevenue = allSubscriptions.Sum(s => s.AmountPaid);
            
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var monthlyRevenue = allSubscriptions
                .Where(s => s.CreatedAt.Month == currentMonth && s.CreatedAt.Year == currentYear)
                .Sum(s => s.AmountPaid);

            // Additional Statistics
            var totalPropertyTypes = await _unitOfWork.PropertyTypeRepo.GetAll()
                .Where(pt => !pt.IsDeleted)
                .CountAsync();
            
            var totalProjects = await _unitOfWork.ProjectRepo.GetAll()
                .Where(pr => !pr.IsDeleted)
                .CountAsync();

            // Calculate Property Types Distribution for Charts
            var propertyTypesWithCounts = await _unitOfWork.PropertyTypeRepo.GetAll()
                .Where(pt => !pt.IsDeleted)
                .Include(pt => pt.Properties)
                .ToListAsync();

            var propertyTypesDistribution = new Dictionary<string, int>();
            foreach (var type in propertyTypesWithCounts)
            {
                var count = type.Properties.Count(p => !p.IsDeleted);
                propertyTypesDistribution[type.Name] = count;
            }

            return new DashboardDto
            {
                // Users
                UsersCount = usersCount,
                ActiveUsersCount = activeUsersCount,
                VerifiedAgentsCount = verifiedAgentsCount,
                UnverifiedAgentsCount = unverifiedAgentsCount,

                // Properties
                AdsCount = adsCount,
                ActivePropertiesCount = activePropertiesCount,
                PendingPropertiesCount = pendingPropertiesCount,
                RejectedPropertiesCount = rejectedPropertiesCount,
                FeaturedPropertiesCount = featuredPropertiesCount,

                // Subscriptions
                ActiveSubscriptionsCount = activeSubscriptionsCount,
                TotalSubscriptionsCount = totalSubscriptionsCount,
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,

                // Additional
                TotalPropertyTypes = totalPropertyTypes,
                TotalProjects = totalProjects,
                PropertyTypesDistribution = propertyTypesDistribution,
                
                // New Statistics
                OwnersCount = ownersCount,
                AgentsCount = agentsCount,
                SoldOrRentedPropertiesCount = soldOrRentedPropertiesCount
            };
        }
    }
}
