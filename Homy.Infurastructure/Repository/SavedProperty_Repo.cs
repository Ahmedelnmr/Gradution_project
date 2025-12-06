using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class SavedProperty_Repo : Genric_Repo<SavedProperty>, ISavedProperty_Repo
    {
        public SavedProperty_Repo(HomyContext context) : base(context)
        {
        }
    }
}