using famenova.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class CartItem 
    {
        public int Id { get; set; }
        public  string ProductName { get; set; } = default!;
        public string? CategoryName { get; set; }
        public string? PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
       

    }
}