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

 //Customer
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = default!;

        public Address ShippingAddress { get; private set; } = default!;
        public string PhoneNumber { get; set; } = default!;

        //Info
        public DateTime OrderDate { get; set; }
        public OrderSource Source { get; set; }
        //Status
        public OrderStatus OrderStatus { get; private set; }
        public PaymentStatus PaymentStatus { get;private set; }
        public PaymentMethod? PaymentMethod { get; private set; }
        //money 
        public decimal  Subtotal { get; private set; }
        public decimal DeliveryFee { get; private set; }
        public decimal Total { get; private set; }

        //
        //public byte[] RowVersion { get; private set; } = default!;

        public ICollection<OrderItem> OrderItems { get; private set; } = new HashSet<OrderItem>();

        public bool IsStockCommitted =>
          OrderStatus is OrderStatus.Processing or OrderStatus.Shipped or OrderStatus.Delivered;
        public bool CanBePaid =>
            OrderStatus == OrderStatus.Pending &&
            PaymentStatus is PaymentStatus.Pending or PaymentStatus.Failed;

        public static Order CreatePending(
           int userId, OrderSource source, Address shippingAddress, string phoneNumber)
        {
            if (shippingAddress is null)
                throw new DomainException("Shipping address is required.");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Phone number is required.");

            return new Order
            {
                UserId = userId,
                Source = source,
                ShippingAddress = shippingAddress with { },   // نسخة خاصة بالـ Order
                PhoneNumber = phoneNumber,
                OrderDate = DateTime.UtcNow,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                PaymentMethod = null,
                DeliveryFee = 0
            };
        }

        public void AddItem(int productId, int quantity, decimal unitPrice)
        {
            EnsureCanBeModified();
            

            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new DomainException("Unit price cannot be negative.");

            if (OrderItems.Any(i => i.ProductId == productId))
                throw new DomainException("this Product is already in the Order");

            OrderItems.Add(new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice
            });
            RecalculateTotals();
        }

      

        public void SetDeliveryFee(decimal fee)
        {
            EnsureCanBeModified();
            DeliveryFee = fee;
            RecalculateTotals();
        }

        //Payment and confirmation 

        public void ConfirmCashOnDelivery()
        {
            EnsureCanBeConfirmed();
            PaymentMethod = Enums.PaymentMethod.CashOnDelivery;
            PaymentStatus = PaymentStatus.Pending;
            OrderStatus = OrderStatus.Processing;
        }

        public void MarkAsPaid()
        {
            EnsureCanBeConfirmed();
            PaymentMethod = Enums.PaymentMethod.Card;
            PaymentStatus = PaymentStatus.Paid;
            OrderStatus = OrderStatus.Processing;
        }
        
        public void MarkPaymentFailed()
        {
            if (!CanBePaid)
                throw new DomainException("payment can only fail for an unpaid pending order");
            PaymentStatus = PaymentStatus.Failed;
        }
        // الدفع الاونلاين نجح بس المخزون خلص قبل التاكيد
        public void CancelDueToStockShortage()
        {
            if (OrderStatus != OrderStatus.Pending)
                throw new DomainException("Only a pending order can be cancelled due to stock shortage.");

            OrderStatus = OrderStatus.Cancelled;
            PaymentStatus = PaymentStatus.Refunded;
        }
        public void Ship()
        {
            if (OrderStatus != OrderStatus.Processing)
                throw new DomainException("only processing orders can be shipped");
            OrderStatus = OrderStatus.Shipped;
        }
        public void Deliver()
        {
            if (OrderStatus != OrderStatus.Shipped)
                throw new DomainException("Only shipped processing can be deliver");
            OrderStatus = OrderStatus.Delivered;

             // الدفع عند الاستلام 
            if (PaymentMethod == Enums.PaymentMethod.CashOnDelivery)
                PaymentStatus = PaymentStatus.Paid;
        }

        public void RefuseDelivery()
        {
            if (OrderStatus != OrderStatus.Shipped)
                throw new DomainException("Only shipped orders can be refused.");

            if (PaymentMethod != Enums.PaymentMethod.CashOnDelivery)
                throw new DomainException("Only cash on delivery orders can be refused at the door.");

            OrderStatus = OrderStatus.Cancelled;
        }
        public void Cancel()
        {
            if (OrderStatus is not (OrderStatus.Pending or OrderStatus.Processing))
                throw new DomainException("Only pending or processing orders can be cancelled.");

            OrderStatus = OrderStatus.Cancelled;
        }

        public void MarkAsRefunded()
        {
            if (OrderStatus != OrderStatus.Cancelled)
                throw new DomainException("Only cancelled orders can be refunded.");

            if (PaymentStatus != PaymentStatus.Paid)
                throw new DomainException("Only paid orders can be refunded.");

            PaymentStatus = PaymentStatus.Refunded;
        }
        private void RecalculateTotals()
        {
            Subtotal = OrderItems.Sum(i => i.UnitPrice * i.Quantity);
            Total = Subtotal + DeliveryFee;
        }

        private void EnsureCanBeConfirmed()
        {
            if (!CanBePaid)
                throw new DomainException("order can only be confirmed while pending and unpaid");

            if (!OrderItems.Any())
                throw new DomainException("Cannot confirm an order without items.");
        }
        private void EnsureCanBeModified()
        {
            if (OrderStatus != OrderStatus.Pending || PaymentStatus == PaymentStatus.Paid)
                throw new DomainException("Order can only be modified while pendng and unpaid");
        }
    }
}
