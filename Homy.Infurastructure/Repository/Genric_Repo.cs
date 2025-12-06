    using Homy.Domin.Contract_Repo;
using Homy.Infurastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Infurastructure.Repository
{
    public class Genric_Repo<T> : IGenric_Repo<T> where T : class
    {
        protected readonly HomyContext context;
        protected readonly DbSet<T> dbset;
        public Genric_Repo(HomyContext homyContext)
        {
            
                context = homyContext;
                dbset = homyContext.Set<T>();
        }
        public IQueryable<T> GetAll()
        {
            return dbset.AsQueryable();  
        }

        Task<T> IGenric_Repo<T>.AddAsync(T item)
        {
            throw new NotImplementedException();
        }

        Task<T> IGenric_Repo<T>.DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<T>> IGenric_Repo<T>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<T> IGenric_Repo<T>.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        void IGenric_Repo<T>.Update(T item)
        {
            throw new NotImplementedException();
        }
    }
}
