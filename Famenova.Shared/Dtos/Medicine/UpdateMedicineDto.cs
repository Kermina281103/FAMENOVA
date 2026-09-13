using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Medicine
{
    public class UpdateMedicineDto
    {
        public string? ActiveIngredient { get; set; }
        public string? DosageForm { get; set; }
        public string? Concentration { get; set; }
        public bool? RequiresPrescription { get; set; }
        public int? Stock { get; set; }

        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
