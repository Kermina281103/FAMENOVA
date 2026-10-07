using famenova.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class Product:BaseAuditableEntity<int>
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Category Category { get; set; } = default!;
        public int CategoryId { get; set; }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            Stock += quantity;
        }

        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (quantity > Stock)
                throw new InvalidOperationException("Insufficient stock.");

            Stock -= quantity;
        }

        public void AdjustStock(int difference)
        {
            if (Stock + difference < 0)
                throw new InvalidOperationException("Stock cannot be negative.");

            Stock += difference;
        }

    }
}
