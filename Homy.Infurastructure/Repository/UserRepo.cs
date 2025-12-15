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
    public class UserRepo : IUserRepo
    {
        private readonly HomyContext _context;

        public UserRepo(HomyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(
                UserRole? role = null,
                bool? isVerified = null,
                bool? isActive = null,
                string searchTerm = null)
        {
            var query = _context.Users.AsQueryable();

            if (role.HasValue)
            {
                query = query.Where(u => u.Role == role.Value);
            }


            if (isVerified.HasValue)
            {
                query = query.Where(u => u.IsVerified == isVerified.Value);
            }


            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(searchTerm) ||
                    u.PhoneNumber.Contains(searchTerm) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchTerm)));
            }

            return await query
                .OrderByDescending(u => u.Id)
                .ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Properties)
                .Include(u => u.SavedProperties)
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        //public async Task<bool> UpdateActiveStatusAsync(Guid userId, bool isActive)
        //{
        //    var user = await _context.Users.FindAsync(userId);
        //    if (user == null)
        //        return false;

        //    user.IsActive = isActive;
        //    _context.Users.Update(user);
            

        //    return true;
        //}

        public async Task<bool> UpdateActiveStatusAsync(Guid userId, bool isActive)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

            user.IsActive = isActive;
            
            return true;
        }

        public async Task<IEnumerable<User>> GetUnverifiedAgentsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Agent && !u.IsVerified)
                .OrderBy(u => u.Id)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetUserStatisticsAsync()
        {
            var stats = new Dictionary<string, int>
            {
                ["TotalUsers"] = await _context.Users.CountAsync(),
                ["ActiveUsers"] = await _context.Users.CountAsync(u => u.IsActive),
                ["TotalOwners"] = await _context.Users.CountAsync(u => u.Role == UserRole.Owner),
                ["TotalAgents"] = await _context.Users.CountAsync(u => u.Role == UserRole.Agent),
                ["VerifiedAgents"] = await _context.Users.CountAsync(u => u.Role == UserRole.Agent && u.IsVerified),
                ["UnverifiedAgents"] = await _context.Users.CountAsync(u => u.Role == UserRole.Agent && !u.IsVerified),
                ["Admins"] = await _context.Users.CountAsync(u => u.Role == UserRole.Admin)
            };

            return stats;
        }

        public async Task<int> CountUserAsync(UserRole? role = null, bool? isVerified = null)
        {
            var query = _context.Users.AsQueryable();

            if (role.HasValue)
            {
                query = query.Where(u => u.Role == role.Value);
            }

            if (isVerified.HasValue)
            {
                query = query.Where(u => u.IsVerified == isVerified.Value);
            }

            return await query.CountAsync();
        }

        public async Task<bool> UpdateVerificationStatusAsync(Guid UserId, bool? isVerified, string? reason = null)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
                return false;

            user.IsVerified = (bool) isVerified;

            _context.Users.Update(user);
            //await _context.SaveChangesAsync();

            return true;  
        }

        public async Task<bool> DeleteUserAsync(Guid UserId)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
                return false;

            user.IsDeleted = true;
            user.IsActive = false;
            _context.Users.Update(user);
            //await _context.SaveChangesAsync();

            return true;
        }
    }
}

