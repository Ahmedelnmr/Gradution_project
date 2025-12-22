using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Microsoft.AspNetCore.Identity;
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

            // Filters
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
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchTerm)));
            }

            // Join with Roles
            var joinedQuery = from u in query
                              join ur in _context.Set<IdentityUserRole<Guid>>() on u.Id equals ur.UserId into userRoles
                              from ur in userRoles.DefaultIfEmpty()
                              join r in _context.Set<IdentityRole<Guid>>() on ur.RoleId equals r.Id into roles
                              from r in roles.DefaultIfEmpty()
                              select new { User = u, RoleName = r.Name };

            // Apply Role Filter if requested
            if (role.HasValue)
            {
                string roleName = role.Value.ToString();
                joinedQuery = joinedQuery.Where(x => x.RoleName == roleName);
            }

            var result = await joinedQuery
                .OrderByDescending(x => x.User.Id)
                .ToListAsync();

            // Map results to User objects with populated Role
            return result.Select(x =>
            {
                if (Enum.TryParse<UserRole>(x.RoleName, out var parsedRole))
                {
                    x.User.Role = parsedRole;
                }
                else
                {
                    x.User.Role = UserRole.Owner; // Default or fallback
                }
                return x.User;
            });
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var query = from u in _context.Users
                        where u.Id == userId
                        join ur in _context.Set<IdentityUserRole<Guid>>() on u.Id equals ur.UserId into userRoles
                        from ur in userRoles.DefaultIfEmpty()
                        join r in _context.Set<IdentityRole<Guid>>() on ur.RoleId equals r.Id into roles
                        from r in roles.DefaultIfEmpty()
                        select new { User = u, RoleName = r.Name };

            var result = await query
                .FirstOrDefaultAsync();

            if (result == null) return null;

            // Load related data explicitly if needed (or use Includes in the query above if possible)
            // Includes are tricky with Projection. 
            // Better to load User first with Includes, then get Role.
            // But we can just use _context.Entry(user).Collection... for lazy loading if enabled, but usually unsafe.
            
            // Let's stick to simple efficient approach: Fetch properties separately or re-attach?
            // Or just Include in the query before projection?
            // "Include" is ignored if we project to anonymous type.
            
            // Re-fetching approach (cleaner code, maybe slightly less performant but fine for ById):
            var user = await _context.Users
                .Include(u => u.Properties)
                .Include(u => u.SavedProperties)
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null && result.RoleName != null)
            {
                 if (Enum.TryParse<UserRole>(result.RoleName, out var parsedRole))
                {
                    user.Role = parsedRole;
                }
            }

            return user;
        }

        public async Task<User> GetUserByPhoneAsync(string phoneNumber)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            
            // We should ideally populate Role here too if needed, but keeping it simple for now as it's mostly for auth checks which we refactored.
            return user;
        }

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
            var query = from u in _context.Users
                        join ur in _context.Set<IdentityUserRole<Guid>>() on u.Id equals ur.UserId
                        join r in _context.Set<IdentityRole<Guid>>() on ur.RoleId equals r.Id
                        where r.Name == "Agent" && !u.IsVerified
                        select new { User = u, RoleName = r.Name };

            var result = await query
                .OrderBy(x => x.User.Id)
                .ToListAsync();

            return result.Select(x =>
            {
                x.User.Role = UserRole.Agent;
                return x.User;
            });
        }

        public async Task<Dictionary<string, int>> GetUserStatisticsAsync()
        {
            // Helper function to count by role
            async Task<int> CountByRole(string roleName)
            {
                return await (from u in _context.Users
                              join ur in _context.Set<IdentityUserRole<Guid>>() on u.Id equals ur.UserId
                              join r in _context.Set<IdentityRole<Guid>>() on ur.RoleId equals r.Id
                              where r.Name == roleName
                              select u).CountAsync();
            }

            // Helper function to count by role and condition
            async Task<int> CountByRoleAndCondition(string roleName, System.Linq.Expressions.Expression<Func<User, bool>> predicate)
            {
                return await (from u in _context.Users
                              join ur in _context.Set<IdentityUserRole<Guid>>() on u.Id equals ur.UserId
                              join r in _context.Set<IdentityRole<Guid>>() on ur.RoleId equals r.Id
                              where r.Name == roleName
                              select u).Where(predicate).CountAsync();
            }

            var stats = new Dictionary<string, int>
            {
                ["TotalUsers"] = await _context.Users.CountAsync(),
                ["ActiveUsers"] = await _context.Users.CountAsync(u => u.IsActive),
                ["TotalOwners"] = await CountByRole("Owner"),
                ["TotalAgents"] = await CountByRole("Agent"),
                ["VerifiedAgents"] = await CountByRoleAndCondition("Agent", u => u.IsVerified),
                ["UnverifiedAgents"] = await CountByRoleAndCondition("Agent", u => !u.IsVerified),
                ["Admins"] = await CountByRole("Admin")
            };

            return stats;
        }

        public async Task<int> CountUserAsync(UserRole? role = null, bool? isVerified = null)
        {
            var query = _context.Users.AsQueryable();

            if (isVerified.HasValue)
            {
                query = query.Where(u => u.IsVerified == isVerified.Value);
            }

            if (role.HasValue)
            {
                string roleName = role.Value.ToString();
                return await (from u in query
                              join ur in _context.Set<IdentityUserRole<Guid>>() on u.Id equals ur.UserId
                              join r in _context.Set<IdentityRole<Guid>>() on ur.RoleId equals r.Id
                              where r.Name == roleName
                              select u).CountAsync();
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
