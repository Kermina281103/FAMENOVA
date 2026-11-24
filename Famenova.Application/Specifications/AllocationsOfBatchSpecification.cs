using famenova.Domain.Entities;
using Famenova.Shared.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Famenova.Application.Specifications
{
    /// <summary>Sales made from one batch, with ALL allocations of each sold line (to re-cost the line)</summary>
    public class AllocationsOfBatchSpecification : BaseSpecification<OrderItemBatch>
    {
        public AllocationsOfBatchSpecification(int batchId)
        {
            Criteria = a => a.MedicineBatchId == batchId;
            AddInclude(q => q.Include(a => a.OrderItem).ThenInclude(i => i.Allocations));
        }
    }
}
