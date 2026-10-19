using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Order
{
   public class OrderListItemDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = default!;
        public string PaymentStatus { get; set; } = default!;
        public string? PaymentMethod { get; set; }
        public string Source { get; set; } = default!;
        public decimal Total { get; set; }
        public string? CustomerName { get; set; }
    }
}
