using famenova.Domain.Common;
using famenova.Domain.Enums;
using famenova.Domain.Exceptions;
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

        public static Order CreatePending(int userId, Address shippingAddress, string phoneNumber)
        {
            return new Order
            {
                UserId = userId,
                ShippingAddress = shippingAddress,
                PhoneNumber = phoneNumber,
                OrderDate = DateTime.UtcNow,
                OrderStatus = OrderStatus.pending,   
                PaymentStatus = PaymentStatus.Pending
            };
        }

        public void AddItem(int productId, int quantity, decimal unitPrice)
        {
            if (OrderStatus != OrderStatus.pending || PaymentStatus != PaymentStatus.Pending)
                throw new DomainException("Items can only be added to a pending, unpaid order.");

            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new DomainException("Unit price cannot be negative.");

            OrderItems.Add(new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice
            });
        }
    }
}
