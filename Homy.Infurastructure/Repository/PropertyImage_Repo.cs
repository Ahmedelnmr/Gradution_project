using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class PropertyImage_Repo : Genric_Repo<PropertyImage>, IPropertyImage_Repo
    {
        public PropertyImage_Repo(HomyContext context) : base(context)
        {
        }
    }
}