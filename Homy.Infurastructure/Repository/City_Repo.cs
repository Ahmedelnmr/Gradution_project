using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class City_Repo : Genric_Repo<City>, ICity_Repo
    {
        public City_Repo(HomyContext context) : base(context)
        {
        }
    }
}   