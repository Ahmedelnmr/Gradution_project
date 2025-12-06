using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class Package_Repo : Genric_Repo<Package>, IPackage_Repo
    {
        public Package_Repo(HomyContext context) : base(context)
        {
        }
    }
}