using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.MedicineBatch
{
    public class UpdateMedicineBatchDto
    {
       
        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int? Quantity { get; set; }
       // public int? MedicineId { get; set; }
    }
}
