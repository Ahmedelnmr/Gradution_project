using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class PropertyType_Repo : Genric_Repo<PropertyType>, IPropertyType_Repo
    {
        public PropertyType_Repo(HomyContext context) : base(context)
        {
        }
    }
}   