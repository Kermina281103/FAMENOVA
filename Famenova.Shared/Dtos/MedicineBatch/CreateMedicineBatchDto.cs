using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.MedicineBatch
{
    public class CreateMedicineBatchDto
    {
        public string BatchNumber { get; set; } = default!;
        public DateTime ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public int MedicineId { get; set; }


    }
}
