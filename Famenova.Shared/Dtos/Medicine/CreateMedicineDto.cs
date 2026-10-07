using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Medicine
{
    public class CreateMedicineDto
    {
        public string? ActiveIngredient { get; set; }
        public string DosageForm { get; set; }
        public string Concentration { get; set; } = default!;
        public bool RequiresPrescription { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}
