using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class Property_Service : IProperty_Service
    {
        private readonly IProperty_Repo _propertyRepo;

        public Property_Service(IProperty_Repo propertyRepo)
        {
            _propertyRepo = propertyRepo;
        }
    }
}