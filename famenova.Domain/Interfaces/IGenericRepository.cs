using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Interfaces
{
   public interface IGenericRepository<TEntity> where TEntity:class
    {
        Task AddAsync(TEntity entity);
        void RemoveAsync(TEntity entity);
        void UpdateAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(int id);
    }
}
