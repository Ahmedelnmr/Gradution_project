using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class Amenity_Repo : Genric_Repo<Amenity>, IAmenity_Repo
    {
        public Amenity_Repo(HomyContext context) : base(context)
        {
        }
    }
}