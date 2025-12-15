using Homy.Domin.Contract_Repo;
using Homy.Domin.models;

namespace Homy.Domin.Contract_Repo
{
    public interface IUserSubscription_Repo : IGenric_Repo<UserSubscription>
    {
        Task<UserSubscription?> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<UserSubscription>> GetByPackageIdAsync(long packageId);
        Task<IEnumerable<UserSubscription>> GetExpiredAsync();
        Task<IEnumerable<UserSubscription>> GetExpiringIn7DaysAsync();
    }
}