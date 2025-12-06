using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class PropertyType_Service : IPropertyType_Service
    {
        private readonly IPropertyType_Repo _propertyTypeRepo;

        public PropertyType_Service(IPropertyType_Repo propertyTypeRepo)
        {
            _propertyTypeRepo = propertyTypeRepo;
        }
    }
}