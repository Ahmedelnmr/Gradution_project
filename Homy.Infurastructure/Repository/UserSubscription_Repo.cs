using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class UserSubscription_Repo : Genric_Repo<UserSubscription>, IUserSubscription_Repo
    {
        public UserSubscription_Repo(HomyContext context) : base(context)
        {
        }
    }
}