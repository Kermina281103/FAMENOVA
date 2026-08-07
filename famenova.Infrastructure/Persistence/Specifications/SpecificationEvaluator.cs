using Famenova.Shared.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Persistence.Specifications
{
    public static class  SpecificationEvaluator
    {
        public static IQueryable<TEntity> GetQuery<TEntity>(IQueryable<TEntity> inputQuery,ISpecification<TEntity> specification,bool applyPaging=true) where TEntity:class
        {
            var query = inputQuery; 
            if(specification.Criteria is not null)
            {
                query = query.Where(specification.Criteria);
            }

            foreach(var include in specification.Includes)
            {
                query = query.Include(include);
            }

            if(specification.OrderBy is not null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            if(specification.OrderByDescending is not null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            if (applyPaging && specification.IsPagingEnabled)
            {
                
                query = query.Skip(specification.Skip)
                    .Take(specification.Take);
            }
            return query;

        }

    }
}
