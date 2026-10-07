using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Common
{
    public class AddressDto
    {
        public string street { get; set; } = default!;
        public string City { get; set; } = default!;
        public string? ZipCode { get; set; }
    }
}
