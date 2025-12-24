 using Homy.Application.Dtos.Admin;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
            var usersCount = await _userManager.Users.CountAsync();
            var adsCount = await _unitOfWork.PropertyRepo.CountAsync();
            
            // Count active properties
            var activePropertiesCount = await _unitOfWork.PropertyRepo.GetAll()
                .Where(p => p.Status == PropertyStatus.Active)
                .CountAsync();
            
            // Count projects
            var projectsCount = await _unitOfWork.ProjectRepo.CountAsync();
            
            // Calculate monthly revenue from subscriptions
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var monthlyRevenue = await _unitOfWork.UserSubscriptionRepo.GetAll()
                .Include(s => s.Package)
                .Where(s => s.StartDate.Month == currentMonth && s.StartDate.Year == currentYear)
                .SumAsync(s => (decimal?)s.Package.Price) ?? 0;

            return new DashboardDto
            {
                UsersCount = usersCount,
                AdsCount = adsCount,
                ActivePropertiesCount = activePropertiesCount,
                MonthlyRevenue = monthlyRevenue,
                ProjectsCount = projectsCount
            };
        }
    }
}
