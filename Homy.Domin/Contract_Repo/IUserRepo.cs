using Homy.Domin.models;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.Contract_Repo
{
    public interface IUserRepo
    {
        Task<IEnumerable<User>> GetAllUsersAsync(
            UserRole? role = null,           
            bool? isVerified = null,         
            bool? isActive = null,           
            string searchTerm = null);

        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByPhoneAsync(string phoneNumber);
        Task<int> CountUserAsync(UserRole? role = null, bool? isVerified = null);
        Task<bool> UpdateVerificationStatusAsync(Guid UserId, bool? isVerified, string? reason = null);
        Task<bool> UpdateActiveStatusAsync(Guid UserId, bool isActive);
        Task<bool> DeleteUserAsync(Guid UserId);
        Task<IEnumerable<User>> GetUnverifiedAgentsAsync();
        Task<Dictionary<string, int>> GetUserStatisticsAsync();
    }
}
