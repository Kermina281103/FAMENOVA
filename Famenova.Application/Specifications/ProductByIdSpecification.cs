using famenova.Domain.Entities;
using Famenova.Shared.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Specifications
{
    public class ProductByIdSpecification:BaseSpecification<Product>
    {
        public ProductByIdSpecification(IEnumerable<int>ProductIds)
        {
            var ids = ProductIds.Distinct().ToList();
            Criteria = c => ids.Contains(c.Id);
        }
    }
}
