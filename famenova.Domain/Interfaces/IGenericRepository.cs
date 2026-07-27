using Famenova.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Interfaces
{
   public interface IGenericRepository<TEntity> where TEntity:class
    {
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(int id);

        Task<PagedResult<TEntity>> GetPagedAsync(PaginationParameters parameters);
    }
}
