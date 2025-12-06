using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

namespace Homy.Infurastructure.Service
{
    public class Project_Service : IProject_Service
    {
        private readonly IProject_Repo _projectRepo;

        public Project_Service(IProject_Repo projectRepo)
        {
            _projectRepo = projectRepo;
        }
    }
}