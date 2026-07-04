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

    }
}
