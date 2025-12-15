using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Homy.Infurastructure.Repository
{
    public class UserSubscription_Repo : Genric_Repo<UserSubscription>, IUserSubscription_Repo
    {
        public UserSubscription_Repo(HomyContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<UserSubscription>> GetAllAsync()
        {
            return await dbset.Include(us => us.User).ToListAsync();
        }

        public async Task<UserSubscription?> GetByUserIdAsync(Guid userId)
        {
            return await dbset.Include(us => us.User).FirstOrDefaultAsync(us => us.UserId == userId);
        }

        public async Task<IEnumerable<UserSubscription>> GetByPackageIdAsync(long packageId)
        {
            return await dbset.Include(us => us.User).Where(us => us.PackageId == packageId).ToListAsync();
        }

        public async Task<IEnumerable<UserSubscription>> GetExpiredAsync()
        {
            var now = DateTime.Now;
            return await dbset.Include(us => us.User).Where(us => us.EndDate <= now).ToListAsync();
        }

        public async Task<IEnumerable<UserSubscription>> GetExpiringIn7DaysAsync()
        {
            var now = DateTime.Now;
            var targetDate = now.AddDays(7);
            return await dbset.Include(us => us.User).Where(us => us.EndDate <= targetDate && us.EndDate > now).ToListAsync();
        }
    }
    }
