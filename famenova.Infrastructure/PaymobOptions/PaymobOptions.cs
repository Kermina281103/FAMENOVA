using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Payments
{
    public class PaymobOptions
    {
        public string SecretKey { get; set; } = string.Empty;

        public string PublicKey { get; set; } = string.Empty;
        public string HmacSecret { get; set; } = string.Empty;

        public int IntegrationId { get; set; }

       
    }
}
