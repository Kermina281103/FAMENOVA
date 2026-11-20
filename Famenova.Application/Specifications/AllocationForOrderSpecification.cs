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
    public class AllocationsForOrderSpecification
      : BaseSpecification<OrderItemBatch>
    {
        public AllocationsForOrderSpecification(
            IEnumerable<int> orderItemIds)
        {
            var ids = orderItemIds.ToList();

            Criteria = a => ids.Contains(a.OrderItemId);

            AddInclude(q =>
                q.Include(a => a.MedicineBatch));
        }
    }
}
