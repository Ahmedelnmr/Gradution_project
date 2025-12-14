using Homy.Domin.models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Domin.Contract_Repo
{
    public interface IPropertyReview_Repo : IGenric_Repo<PropertyReview>
    {
        Task<IEnumerable<PropertyReview>> GetByPropertyIdAsync(long propertyId);
    }
}
