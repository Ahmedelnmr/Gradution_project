using Homy.Domin.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Domin.Contract_Service
{
    public interface IPropertyReview_Service
    {
        Task<IEnumerable<PropertyReview>> GetAllReviewsAsync();
        Task<PropertyReview> GetReviewByIdAsync(int id);
        Task<bool> DeleteReviewAsync(int id);
    }
}
