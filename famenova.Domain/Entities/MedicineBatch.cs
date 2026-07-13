using famenova.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class MedicineBatch:BaseEntity<int>
    {
        public string BatchNumber { get; set; } = default!;
        public DateTime ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; } = default!;
    }
}
