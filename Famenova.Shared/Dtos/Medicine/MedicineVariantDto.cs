using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Medicine
{
    public class MedicineVariantDto
    {
        public int Id { get; set; }
        public string VariantLabel { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Concentration { get; set; }            // التركيز
        public bool RequiresPrescription { get; set; }

    }
}
