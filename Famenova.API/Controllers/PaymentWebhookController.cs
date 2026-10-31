using Azure.Core;
using Famenova.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/webhooks")]
    public class PaymentWebhookController(IPaymentWebhookService _webhookService) :ControllerBase
    {
        [HttpPost("payments")]
        [AllowAnonymous]
        public async Task<IActionResult> Payments()
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();
            var signature = Request.Query["hmac"].ToString();

            await _webhookService.HandleAsync(rawBody, signature);
            return Ok();
        }
    }
}
