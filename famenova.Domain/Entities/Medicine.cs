using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class Medicine:Product
    {
        public string? ActiveIngredient { get; set; }
        public string DosageForm { get; set; } = default!;
        public string Concentration { get; set; } = default!;
        public bool RequiresPrescription { get; set; }
        public ICollection<MedicineBatch> Batches { get; set; } = new HashSet<MedicineBatch>();

    }
}
