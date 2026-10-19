using Famenova.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Order
{
    public class CreateOrderFromCartDto
    {
        public string CartId { get; set; } = default!;
        public AddressDto ShippingAddress { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;

    }
}
