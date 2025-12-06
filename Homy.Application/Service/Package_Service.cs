using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class Package_Service : IPackage_Service
    {
        private readonly IPackage_Repo _packageRepo;

        public Package_Service(IPackage_Repo packageRepo)
        {
            _packageRepo = packageRepo;
        }
    }
}