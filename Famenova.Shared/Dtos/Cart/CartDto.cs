using Famenova.Shared.Dtos.Cart;
using System.ComponentModel.DataAnnotations;

public class CartDto
{
  
    public  string? Id { get; set; }

    public IEnumerable<CartItemDto> CartItems { get; set; }
        = new List<CartItemDto>();
}