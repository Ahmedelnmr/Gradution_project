using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class District_Service : IDistrict_Service
    {
        private readonly IDistrict_Repo _districtRepo;

        public District_Service(IDistrict_Repo districtRepo)
        {
            _districtRepo = districtRepo;
        }
    }
}