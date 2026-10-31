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
    public class Payment:BaseAuditableEntity<int>
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public decimal Amount { get; private set; }
        public PaymentAttemptStatus Status { get; private set; }

        public string MerchantRefNumber { get; set; } = default!;
        public string? ProvierderReference { get; set; }
        public string? FailureReason { get; set; }
        public DateTime? CompleteAt { get; set; }

        public bool RequiredRefund { get; private set; }
        public string? RefundReason { get; private set; }

        private Payment() { }

        public static Payment Create(int orderId,decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Payment amount must be greater than or equal  0");

            return new Payment()
            {
                OrderId = orderId,
                Amount = amount,
                Status = PaymentAttemptStatus.Pending,
                MerchantRefNumber = Guid.NewGuid().ToString("N")
            };
        }

        public void SetProvideReference(string? reference)
        {
            ProvierderReference = reference;
        }

        public void MarkSucceeded(string? provideReference)
        {
            EnsurePending();
            Status = PaymentAttemptStatus.Succeeded;
            ProvierderReference = provideReference ?? ProvierderReference;
            CompleteAt = DateTime.UtcNow;
        }

       public void MarkFailed(string reason)
        {
            EnsurePending();
            Status = PaymentAttemptStatus.Failed;
            FailureReason = reason;
            CompleteAt = DateTime.UtcNow;
        }
        public void MarkExpired()
        {
            EnsurePending();
            Status = PaymentAttemptStatus.Expired;
            FailureReason = "Payment reference expired.";
            CompleteAt = DateTime.UtcNow;
        }

        public void FlagForRefund(string reason)
        {
            if (Status != PaymentAttemptStatus.Succeeded)
                throw new DomainException("Only succeeded payments can be refunded");
            RequiredRefund = true;
            RefundReason = reason;
        }
        private void EnsurePending()
        {
            if (Status != PaymentAttemptStatus.Pending)
                throw new DomainException("payment has already been completed  ");
        }
    }
}
