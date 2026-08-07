using famenova.Domain.Interfaces;
using famenova.Infrastructure.Data.Context;
using famenova.Infrastructure.Persistence.Specifications;
using Famenova.Application.Common.Models;
using Famenova.Shared.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity>(AppDbContext _context) : IGenericRepository<TEntity> where TEntity:class
    {
        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<PagedResult<TEntity>> GetPagedAsync(ISpecification<TEntity> specification)
        {
            var countQuery = SpecificationEvaluator.GetQuery(_context.Set<TEntity>(), specification, false);

            var totalCount = await countQuery.CountAsync();
            var itemsQuery = SpecificationEvaluator.GetQuery(_context.Set<TEntity>(), specification);
            var items = await itemsQuery.ToListAsync();
            return new PagedResult<TEntity>(items, specification.PageIndex, specification.PageSize, totalCount);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TEntity> specification)
        {
            var query = SpecificationEvaluator.GetQuery(_context.Set<TEntity>(), specification);
            return await query.ToListAsync();
        }

        public  void  Remove(TEntity entity)
        {
             _context.Set<TEntity>().Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }
    }
}
