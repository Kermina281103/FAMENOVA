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

    }
}
