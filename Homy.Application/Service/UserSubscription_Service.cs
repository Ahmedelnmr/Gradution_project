using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class UserSubscription_Service : IUserSubscription_Service
    {
        private readonly IUserSubscription_Repo _userSubscriptionRepo;

        public UserSubscription_Service(IUserSubscription_Repo userSubscriptionRepo)
        {
            _userSubscriptionRepo = userSubscriptionRepo;
        }
    }
}