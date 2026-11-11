using famenova.Domain.Constants;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Shared.Dtos.Cart;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Customer)]
    public class CartController(ICartService cartService):ControllerBase
    {
        [HttpGet]
       
        public async Task<ActionResult<CartDto>> GetByIdAsync(string cartId)
        {
           
            var uid = User.GetUserId();
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
