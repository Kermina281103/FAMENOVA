using famenova.Domain.Common;
using famenova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class Order:BaseAuditableEntity<int>
    {
        public ApplicationUser User { get; set; } = default!;
        public int UserId { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public Address ShippingAddress { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus  PaymentStatus { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();


    }
}
