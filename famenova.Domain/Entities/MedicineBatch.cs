using famenova.Domain.Common;
using famenova.Domain.Exceptions;
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
        public void Consume(int quantity)
        {
            if (quantity <= 0 || quantity > Quantity)
                throw new DomainException(
                    "Invalid quantity for this batch.");

            Quantity -= quantity;
        }

        public void Restore(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException(
                    "Quantity must be greater than zero.");

            Quantity += quantity;
        }
    }
}
