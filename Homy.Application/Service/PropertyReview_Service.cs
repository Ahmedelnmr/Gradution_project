using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Infurastructure.Service
{
    public class PropertyReview_Service : IPropertyReview_Service
    {
        private readonly IPropertyReview_Repo _reviewRepo;
        private readonly IUnitofwork _unitofwork;

        public PropertyReview_Service(IPropertyReview_Repo reviewRepo, IUnitofwork unitofwork)
        {
            _reviewRepo = reviewRepo;
            _unitofwork = unitofwork;
        }

        public async Task<IEnumerable<PropertyReview>> GetAllReviewsAsync()
        {
             return await _reviewRepo.GetAll()
                 .Include(x => x.Admin)
                 .Include(x => x.Property)
                 .OrderByDescending(x => x.CreatedAt)
                 .ToListAsync();
        }

        public async Task<PropertyReview> GetReviewByIdAsync(int id)
        {
            return await _reviewRepo.GetByIdAsync(id);
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _reviewRepo.GetByIdAsync(id);
            if (review == null) return false;

            await _reviewRepo.DeleteAsync(id);
            await _unitofwork.Save();
            return true;
        }
    }
}
