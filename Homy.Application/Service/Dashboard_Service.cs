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
            var usersCount = await _userManager.Users.CountAsync();
            var adsCount = await _unitOfWork.PropertyRepo.CountAsync();

            return new DashboardDto
            {
                UsersCount = usersCount,
                AdsCount = adsCount
            };
        }
    }
}
