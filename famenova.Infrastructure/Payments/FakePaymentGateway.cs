using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Payments
{

    public class FakeGatewayOptions
    {
        public string SecureKey { get; set; } = default!;
    }
    public class FakeNotificationPayload
    {
        public string merchantRefNumber { get; set; } = default!;
        public string? providerRefNumber { get; set; }
        public string orderStatus { get; set; } = default!;
        public decimal paymentAmount { get; set; }
        public string? failureReason { get; set; }
        public string messageSignature { get; set; } = default!;


    }
   
}
