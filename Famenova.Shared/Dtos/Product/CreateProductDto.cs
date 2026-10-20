using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Product
{
    public class CreateProductDto
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int stock { get; set; } 

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}
