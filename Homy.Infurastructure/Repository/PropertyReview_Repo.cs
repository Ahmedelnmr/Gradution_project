using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Infurastructure.Repository
{
    public class PropertyReview_Repo : Genric_Repo<PropertyReview>, IPropertyReview_Repo
    {
        public PropertyReview_Repo(HomyContext context) : base(context) { }

        public async Task<IEnumerable<PropertyReview>> GetByPropertyIdAsync(long propertyId)
        {
            return await dbset
                .Include(r => r.Admin)
                .Where(r => r.PropertyId == propertyId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
