using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class PropertyImage_Service : IPropertyImage_Service
    {
        private readonly IPropertyImage_Repo _propertyImageRepo;

        public PropertyImage_Service(IPropertyImage_Repo propertyImageRepo)
        {
            _propertyImageRepo = propertyImageRepo;
        }
    }
}