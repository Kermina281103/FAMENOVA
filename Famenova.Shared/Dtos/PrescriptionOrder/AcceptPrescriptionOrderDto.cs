using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.PrescriptionOrder
{
    class AcceptPrescriptionOrderDto
    {
        public ICollection<CreatePrescriptionOrderItemDto> Items { get; set; }
       = new List<CreatePrescriptionOrderItemDto>();
    }
}
