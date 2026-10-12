using Famenova.Shared.Dtos.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string cartId);
        Task<CartDto> UpdateCartAsync(CartDto dto);
        Task DeleteCartAsync(string cartId);
    }
}
