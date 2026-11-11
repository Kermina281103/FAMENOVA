using famenova.Domain.Constants;
using famenova.Domain.Enums;
using Famenova.Application.Common.Models;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Application.Specifications.param;
using Famenova.Shared.Dtos.Order;
using Famenova.Shared.Dtos.Pay;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderService _orderService):ControllerBase
    {
        [HttpPost("createWithCart")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> CreateAynsc(CreateOrderFromCartDto dto)
        {
            var uid = User.GetUserId();

            var result = await _orderService.CreateOrderFromCartAsync(uid, dto);
            return Ok(ApiResponse<OrderDetailsDto>.Success(result, "Order Created Successfully"));
        }

      // customer 

        [HttpGet("my")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles =AppRoles.Customer)]
        public async Task<ActionResult<ApiResponse<PagedResult<OrderListItemDto>>>> GetMyOrdersAsync([FromQuery]OrderSpecParams spec)
        {
            var uid = User.GetUserId();

            var result = await _orderService.GetMyOrdersAsync(spec, uid);

            return Ok(ApiResponse<PagedResult<OrderListItemDto>>.Success(result));
        }

        [HttpGet("myWithId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Customer)]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> GetMyOrderWithIdAsync(int id )
        {
            var uid = User.GetUserId();
            var result = await _orderService.GetMyOrderWithIdAsync(id, uid);

            return Ok(ApiResponse<OrderDetailsDto>.Success(result));
        }
        [HttpPost("pay")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Customer)]
        public async Task<ActionResult<ApiResponse<PayOrderResultDto>>> PayMy(int id, PaymentMethod  paymentMethod)
        {
            var uid = User.GetUserId();
            var result = await _orderService.PayMyOrderAsync(id, uid, paymentMethod);
            return Ok(ApiResponse<PayOrderResultDto>.Success(result, "Order confirmed successfully"));
        }
    
        [HttpPost("cancel")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Customer)]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> CancelMyOrder(int id, int userId)
        {
            var uid = User.GetUserId();
            var result = await _orderService.CancelMyOrderAsync(id, uid);
            return Ok(ApiResponse<OrderDetailsDto>.Success(result, "Order cancelled successfully"));
        }


        [HttpGet("getAll")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Admin)]

        public async Task<ActionResult<ApiResponse<PagedResult<OrderListItemDto>>>> GetAllAsync([FromQuery] OrderSpecParams spec)
        {
            var result = await _orderService.GetAllAsync(spec);
            return Ok(ApiResponse<PagedResult<OrderListItemDto>>.Success(result));
        }


        [HttpGet("getWithId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Admin)]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> GetOrderWithId(int id)
        {
            var result = await _orderService.GetByIdAsync(id);

            return Ok(ApiResponse<OrderDetailsDto>.Success(result));
        }

    }
}
