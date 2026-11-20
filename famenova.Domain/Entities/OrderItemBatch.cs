using famenova.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class OrderItemBatch : BaseEntity<int>
    {
        public int OrderItemId { get; set; }

        public OrderItem OrderItem { get; set; } = default!;

        public int MedicineBatchId { get; set; }

        public MedicineBatch MedicineBatch { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
