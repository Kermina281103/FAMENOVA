using Famenova.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Order
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        public string OrderStatus { get; set; } = default!;
        public string PaymentStatus { get; set; } = default!;
        public string? PaymentMethod { get; set; }
        public string Source { get; set; } = default!;

        public AddressDto ShippingAddress { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;

        public int UserId { get; set; }
        public string? CustomerName { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Total { get; set; }

        public bool CanBePaid { get; set; }   // الفرونت يقرر يظهر زرار الدفع ولا لأ

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
