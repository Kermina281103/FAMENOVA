using Famenova.Shared.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Pay
{
    public class PayOrderResultDto
    {
        public OrderDetailsDto Order { get; set; } = default!;
        public string? PaymentUrl { get; set; }          
        public string? MerchantRefNumber { get; set; }
    }
}
