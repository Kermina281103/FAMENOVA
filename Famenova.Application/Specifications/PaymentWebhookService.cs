using famenova.Domain.Entities;
using famenova.Domain.Enums;
using famenova.Domain.Exceptions;
using famenova.Domain.Interfaces;
using Famenova.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Specifications
{
    public class PaymentWebhookService : IPaymentWebhookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateWay _gateway;
        private readonly IOrderConfirmationService _confirmation;
        private readonly ILogger<PaymentWebhookService> _logger;

        public PaymentWebhookService(
            IUnitOfWork unitOfWork,
            IPaymentGateWay gateway,
            IOrderConfirmationService confirmation,
            ILogger<PaymentWebhookService> logger)
        {
            _unitOfWork = unitOfWork;
            _gateway = gateway;
            _confirmation = confirmation;
            _logger = logger;
        }
        public async Task HandleAsync(string rowBody, string? Signature)
        {
            var notification = _gateway.ParseNotification(rowBody, Signature);

            if (notification.Result == GatewayPaymentResult.Ignored)
                return;

            if (string.IsNullOrEmpty(notification.MerchantRefNumber))
            {
                _logger.LogWarning("Webhook without a merchant reference");
                return;
            }

            var spec = new PaymentByMerchantRefSpecification(notification.MerchantRefNumber);
            var payment = await _unitOfWork.GetGeneric<Payment>().GetByIdAsync(spec);

            if (payment is null)
            {
                _logger.LogWarning("Webhook for unknown payment{Ref}", notification.MerchantRefNumber);
                return;
            }
            if (payment.Status != PaymentAttemptStatus.Pending)
            {
                _logger.LogInformation("Duplicate webhook for payment {Ref}, already {Status}",
                    payment.MerchantRefNumber, payment.Status);
                return;
            }

            if (notification.Result == GatewayPaymentResult.Paid && notification.Amount != payment.Amount)
            {
                _logger.LogError("Amount mismatch for {Ref}: expected {Expected}, got {Actual}",
                    payment.MerchantRefNumber, payment.Amount, notification.Amount);

                payment.MarkSucceeded(notification.ProviderReference);
                payment.FlagForRefund($"Amount mismatch: expected {payment.Amount}, got {notification.Amount}.");
                await _unitOfWork.SaveChangesAsync();
                return;
            }

            switch (notification.Result)
            {
                case GatewayPaymentResult.Paid:
                    await HandlePaidAsync(payment, notification);
                    break;

                case GatewayPaymentResult.Failed:
                    payment.MarkFailed(notification.FailureReason ?? "Payment failed.");
                    if (payment.Order.CanBePaid) payment.Order.MarkPaymentFailed();
                    break;

                case GatewayPaymentResult.Expired:
                    payment.MarkExpired();
                    if (payment.Order.CanBePaid) payment.Order.MarkPaymentFailed();
                    break;
            }


            await _unitOfWork.SaveChangesAsync();

        }

        private async Task HandlePaidAsync(Payment payment, PaymentNotification notification)
        {
            var order = payment.Order;

           
            payment.MarkSucceeded(notification.ProviderReference);

            
            if (!order.CanBePaid)
            {
                payment.FlagForRefund($"Order is no longer payable (status: {order.OrderStatus}).");
                return;
            }

            try
            {
                
                await _confirmation.ConfirmAsync(order, PaymentMethod.Card);
            }
            catch (DomainException ex)
            {
                
                order.CancelDueToStockShortage();
                payment.FlagForRefund(ex.Message);
            }
        }
    }
}