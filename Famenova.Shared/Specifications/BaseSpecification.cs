using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Specifications
{
    public class BaseSpecification<TEntity> : ISpecification<TEntity>
    where TEntity:class    
    {
        public Expression<Func<TEntity, bool>> Criteria { get; protected set; }

        public IList<Expression<Func<TEntity, object>>> Includes { get; } = new List<Expression<Func<TEntity, object>>>();

        public Expression<Func<TEntity, object>>? OrderBy { get; protected set; }

        public Expression<Func<TEntity, object>>? OrderByDescending { get; protected set; }
        public int PageIndex { get; protected set; }
        public int  PageSize { get; protected set ; }

        public int Skip { get; protected set; }

        public int Take { get; protected set; }

        public bool IsPagingEnabled { get; protected set; }


        protected void AddInclude(Expression<Func<TEntity,object>> include)
        {
            Includes.Add(include);
        }

        protected void AddOrderBy(Expression<Func<TEntity,object>> orderBy)
        {
            OrderBy = orderBy;
        }
        protected void AddOrderByDescending(Expression<Func<TEntity,object>> orderByDescending)
        {
            OrderByDescending = orderByDescending;
        }

        protected void ApplyPaging(int pageIndex , int pageSize)
        {

            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 10;

            PageIndex = pageIndex;
            PageSize = pageSize;

            Skip = (pageIndex - 1) * pageSize;
            Take = pageSize;

            IsPagingEnabled = true;
        }
    }
}
