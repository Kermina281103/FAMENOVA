using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface IPaymentGateWay
    {
        Task<PaymentInitResult> CreatePaymentAsync(PaymentInitRequest request);

        PaymentNotification ParseNotification(string rawBody,string? signature);
    }

    public record PaymentInitRequest(
        string MerchantRefNumber,
        decimal Amount , 
        int CustomerId,
        string CustomerFirstName,
        string CustomerLastName,
        string CustomerEmail,
        string CustomerPhone , 
         string Street,
         string City,
        IReadOnlyList<PaymentInitItem> Items);

    public record PaymentInitItem(int productId, string Name , int Quantity, decimal UnitPrice );

    public record PaymentInitResult(string PaymentUrl, string? ProviderReference);
    public enum GatewayPaymentResult { Paid , Failed, Expired,Ignored}

    public record PaymentNotification(
        string? MerchantRefNumber,
     string? ProviderReference,
     GatewayPaymentResult Result,
    decimal Amount,
    string? FailureReason);

  
}
