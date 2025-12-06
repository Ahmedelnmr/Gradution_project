using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Domin.Contract_Repo
{
    public interface IGenric_Repo<T> where T : class
    {
        public  Task<IEnumerable<T>> GetAllAsync(); 
        public Task<T> GetByIdAsync(int id);
        public Task<T>AddAsync(T item);
        public  void Update(T item);
        public Task<T> DeleteAsync(int id);


        

        
    }
}
