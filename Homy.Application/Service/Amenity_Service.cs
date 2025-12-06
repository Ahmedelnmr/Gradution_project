using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class Amenity_Service : IAmenity_Service
    {
        private readonly IAmenity_Repo _amenityRepo;

        public Amenity_Service(IAmenity_Repo amenityRepo)
        {
            _amenityRepo = amenityRepo;
        }
    }
}