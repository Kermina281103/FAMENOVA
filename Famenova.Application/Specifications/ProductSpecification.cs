using famenova.Domain.Entities;
using Famenova.Shared.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Specifications
{
    public class  ProductSpecification:BaseSpecification<Product>
    {
        public ProductSpecification(string? search, int? categoryId, string? sort, int pageIndex, int pageSize)
        {
            var trim = search?.Trim();
            Criteria = c => (!categoryId.HasValue || c.CategoryId == categoryId) &&
            (string.IsNullOrEmpty(trim) || c.Description!.Contains(trim) || c.Name.Contains(trim));

            AddInclude(query => query.Include(c => c.Category));

            switch (sort?.ToLower())
            {
                case "price": AddOrderBy(x => x.Price);break;
                case "pricedesc":AddOrderByDescending(x => x.Price);break;
                case "category":AddOrderBy(x => x.CategoryId);break;
                case "name":AddOrderBy(x => x.Name);break;
                case "namedesc":AddOrderByDescending(x => x.Name);break;
                default: AddOrderBy(x => x.Name);break;
            }

            ApplyPaging(pageIndex, pageSize);


        }

        public ProductSpecification(int id )
        {
            Criteria = c => c.Id == id;
            AddInclude(query => query.Include(x => x.Category));
        }
    }
}
