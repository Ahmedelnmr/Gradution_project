using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class District_Repo : Genric_Repo<District>, IDistrict_Repo
    {
        public District_Repo(HomyContext context) : base(context)
        {
        }
    }
}