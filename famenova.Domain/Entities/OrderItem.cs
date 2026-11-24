using famenova.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class OrderItem:BaseAuditableEntity<int>
    {
        public Product? Product { get; set; }
        public int ProductId { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// Cost of goods for this line, fixed when the order is confirmed (stock committed):
        /// the cost of the batches actually consumed (medicines) or Product.PurchaseCost x quantity.
        /// Null = the cost was unknown, so this line is left out of profit.
        /// </summary>
        public decimal? CostAmount { get; set; }
        public ICollection<OrderItemBatch> Allocations { get; set; }
    = new HashSet<OrderItemBatch>();

    }
}
