using Famenova.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Order
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public AddressDto ShippingAddress { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;

        public string OrderStatus { get; set; } = default!;
        public string PaymentStatus { get; set; } = default!;

        public List<OrderItemResponseDto> Items { get; set; } = new();
        public decimal Total { get; set; }
    }
}
