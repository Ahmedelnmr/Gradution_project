using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class City_Service : ICity_Service
    {
        private readonly ICity_Repo _cityRepo;

        public City_Service(ICity_Repo cityRepo)
        {
            _cityRepo = cityRepo;
        }
    }
}