using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using famenova.Infrastructure.Payments;

namespace famenova.Infrastructure.Payments
{
    public class PaymobPaymentGateway : IPaymentGateWay
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobOptions _options;

        public PaymobPaymentGateway(HttpClient httpClient, IOptions<PaymobOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<PaymentInitResult> CreatePaymentAsync(PaymentInitRequest request)
        {
            var amountInCents = ToCents(request.Amount);

          
            var items = request.Items.Select(item => new
            {
                name = item.Name,
                amount = ToCents(item.UnitPrice),
                description = item.Name,
                quantity = item.Quantity
            }).ToArray();

           
            var itemsTotalInCents = items.Sum(i => i.amount * i.quantity);
            if (amountInCents != itemsTotalInCents)
            {
                throw new InvalidOperationException(
                    $"Amount mismatch: {amountInCents} vs {itemsTotalInCents}");
            }

            var payload = new
            {
                amount = amountInCents,
                currency = "EGP",
                payment_methods = new[] { _options.IntegrationId },
                items = items,                         
                billing_data = new
                {
                    first_name = request.CustomerFirstName,
                    last_name = request.CustomerLastName,
                    email = request.CustomerEmail,
                    phone_number = request.CustomerPhone,
                    apartment = "NA",
                    floor = "NA",
                    street = request.Street,
                    building = "NA",
                    city = request.City,
                    country = "EG",
                    state = "Cairo"
                },
                special_reference = request.MerchantRefNumber
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/intention/");
            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Token", _options.SecretKey);
            httpRequest.Content = JsonContent.Create(payload);

            using var response = await _httpClient.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Paymob intention request failed. " +
                    $"Status: {(int)response.StatusCode}. " +
                    $"Response: {responseBody}");
            }

            var intention = JsonSerializer.Deserialize<PaymobIntentionResponse>(
                responseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (intention is null || string.IsNullOrWhiteSpace(intention.ClientSecret))
            {
                throw new InvalidOperationException(
                    "Paymob did not return a valid client secret.");
            }

            var checkoutUrl =
                "https://accept.paymob.com/unifiedcheckout/" +
                $"?publicKey={Uri.EscapeDataString(_options.PublicKey)}" +
                $"&clientSecret={Uri.EscapeDataString(intention.ClientSecret)}";

            return new PaymentInitResult(checkoutUrl, intention.Id);
        }




        public PaymentNotification ParseNotification(string rawBody, string? signature)
        {
            using var doc = JsonDocument.Parse(rawBody);

            if (!doc.RootElement.TryGetProperty("obj", out var obj))
                throw new BadRequestException("Invalid Paymob notification.");

            var concatenated = string.Concat(
                Read(obj, "amount_cents"),
                Read(obj, "created_at"),
                Read(obj, "currency"),
                Read(obj, "error_occured"),
                Read(obj, "has_parent_transaction"),
                Read(obj, "id"),
                Read(obj, "integration_id"),
                Read(obj, "is_3d_secure"),
                Read(obj, "is_auth"),
                Read(obj, "is_capture"),
                Read(obj, "is_refunded"),
                Read(obj, "is_standalone_payment"),
                Read(obj, "is_voided"),
                Read(obj, "order", "id"),
                Read(obj, "owner"),
                Read(obj, "pending"),
                Read(obj, "source_data", "pan"),
                Read(obj, "source_data", "sub_type"),
                Read(obj, "source_data", "type"),
                Read(obj, "success"));

            var hash = HMACSHA512.HashData(
                Encoding.UTF8.GetBytes(_options.HmacSecret),
                Encoding.UTF8.GetBytes(concatenated));
            var expected = Convert.ToHexString(hash).ToLowerInvariant();

            if (string.IsNullOrEmpty(signature) || !PaymentSignature.AreEqual(expected, signature))
                throw new BadRequestException("Invalid notification signature.");

            var success = obj.GetProperty("success").GetBoolean();
            var pending = obj.GetProperty("pending").GetBoolean();
            var refunded = obj.GetProperty("is_refunded").GetBoolean();
            var voided = obj.GetProperty("is_voided").GetBoolean();

            var result =
          refunded || voided ? GatewayPaymentResult.Ignored :
          pending ? GatewayPaymentResult.Ignored :
          success ? GatewayPaymentResult.Paid :
                               GatewayPaymentResult.Failed;

            var amount = obj.GetProperty("amount_cents").GetInt64() / 100m;

            return new PaymentNotification(
                Read(obj, "order", "merchant_order_id"),   
                Read(obj, "id"),
                result,
                amount,
                success ? null : "Payment was declined.");
        }

        
        private static string Read(JsonElement element, params string[] path)
        {
            foreach (var key in path)
            {
                if (element.ValueKind != JsonValueKind.Object ||
                    !element.TryGetProperty(key, out element))
                    return "";
            }

            return element.ValueKind switch
            {
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.String => element.GetString() ?? "",
                JsonValueKind.Number => element.GetRawText(),
                _ => ""
            };
        }
        private static long ToCents(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            return checked((long)(
                decimal.Round(
                    amount * 100m,
                    0,
                    MidpointRounding.AwayFromZero)));
        }

        private sealed class PaymobIntentionResponse
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }
            [JsonPropertyName("client_secret")]
            public string? ClientSecret { get; set; }
        }
    }
}
