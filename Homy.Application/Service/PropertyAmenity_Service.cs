using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class PropertyAmenity_Service : IPropertyAmenity_Service
    {
        private readonly IPropertyAmenity_Repo _propertyAmenityRepo;

        public PropertyAmenity_Service(IPropertyAmenity_Repo propertyAmenityRepo)
        {
            _propertyAmenityRepo = propertyAmenityRepo;
        }
    }
}