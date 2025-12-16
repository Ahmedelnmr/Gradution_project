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

        public async Task<int> CountAsync()
        {
            return await dbset.CountAsync();
        }

        public async Task<T> AddAsync(T item)
        {
            await dbset.AddAsync(item);
            return item;
        }

        public async Task<T> DeleteAsync(long id)
        {
            var entity = await dbset.FindAsync(id);
            if (entity != null)
            {
                dbset.Remove(entity);
            }
            return entity;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbset.ToListAsync();
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await dbset.FindAsync(id);
        }

        public void Update(T item)
        {
            dbset.Update(item);
        }
    }
}
