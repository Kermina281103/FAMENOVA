using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Shared.Dtos.Cart;
using Microsoft.AspNetCore.Mvc;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(ICartService cartService):ControllerBase
    {
        [HttpGet]

        public async Task<ActionResult<CartDto>> GetByIdAsync(string cartId)
        {
            var result = await cartService.GetCartAsync(cartId);
            return Ok(ApiResponse<CartDto>.Success(result));
        }

        [HttpPost]
        public async Task<ActionResult<CartDto>> UpdateCartAsync(CartDto dto)
        {
            var result = await cartService.UpdateCartAsync(dto);
            return Ok(ApiResponse<CartDto>.Success(result, "Cart Updted Sucessfully "));
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteCartAsync(string cartId)
        {
            await cartService.DeleteCartAsync(cartId);
            return Ok();

        }

    }
}
