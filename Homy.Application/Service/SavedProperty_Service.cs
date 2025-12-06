using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class SavedProperty_Service : ISavedProperty_Service
    {
        private readonly ISavedProperty_Repo _savedPropertyRepo;

        public SavedProperty_Service(ISavedProperty_Repo savedPropertyRepo)
        {
            _savedPropertyRepo = savedPropertyRepo;
        }
    }
}