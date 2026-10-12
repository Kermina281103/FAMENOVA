using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Cart
{
    public class CartItemDto
    {
        
        public int Id { get; set; }
        [Required]
        public required string ProductName { get; set; } = default!;
        
        public string? PictureUrl { get; set; }
        [Required]
        [Range(0.1,double.MaxValue,ErrorMessage ="Price Must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(1,int.MaxValue,ErrorMessage =("Quantity Must be Greatuer than 1"))]
        public int Quantity { get; set; }
        public string? CategoryName { get; set; } = default!;
        
    }
}
