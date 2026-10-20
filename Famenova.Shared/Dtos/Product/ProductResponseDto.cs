using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Product
{
    public class ProductResponseDto
    {
        public int ID { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string CategoryName { get; set; } = default!;
    }
}
