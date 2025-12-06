using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;

namespace Homy.Infurastructure.Repository
{
    public class Project_Repo : Genric_Repo<Project>, IProject_Repo
    {
        public Project_Repo(HomyContext context) : base(context)
        {
        }
    }
}