using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class PropertyAmenity_Repo : Genric_Repo<PropertyAmenity>, IPropertyAmenity_Repo
    {
        public PropertyAmenity_Repo(HomyContext context) : base(context)
        {
        }
    }
}