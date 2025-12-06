using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class Property_Repo : Genric_Repo<Property>, IProperty_Repo
    {
        public Property_Repo(HomyContext context) : base(context)
        {
        }
    }
}