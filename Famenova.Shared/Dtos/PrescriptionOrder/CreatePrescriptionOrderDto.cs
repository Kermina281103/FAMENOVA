using Famenova.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.PrescriptionOrder
{
    public class CreatePrescriptionOrderDto
    {
      
            public string PrescriptionImageUrl { get; set; } = default!;
            public AddressDto DeliveryAddress { get; set; } = default!;
            public string PhoneNumber { get; set; } = default!;
       

    }
}
