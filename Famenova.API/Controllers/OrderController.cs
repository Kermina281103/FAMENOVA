using famenova.Domain.Enums;
using Famenova.Application.Common.Models;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Application.Specifications.param;
using Famenova.Shared.Dtos.Order;
using Famenova.Shared.Dtos.Pay;
using Microsoft.AspNetCore.Mvc;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderService _orderService):ControllerBase
    {
        [HttpPost("createWithCart")]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> CreateAynsc(int userId,CreateOrderFromCartDto dto)
        {
            var result = await _orderService.CreateOrderFromCartAsync(userId, dto);
            return Ok(ApiResponse<OrderDetailsDto>.Success(result, "Order Created Successfully"));
        }

      // customer 

        [HttpGet("my")]
        public async Task<ActionResult<ApiResponse<PagedResult<OrderListItemDto>>>> GetMyOrdersAsync([FromQuery]OrderSpecParams spec,int userId)
        {
            var result = await _orderService.GetMyOrdersAsync(spec, userId);

            return Ok(ApiResponse<PagedResult<OrderListItemDto>>.Success(result));
        }

        [HttpGet("myWithId")]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> GetMyOrderWithIdAsync(int id , int userId)
        {
            var result = await _orderService.GetMyOrderWithIdAsync(id, userId);

            return Ok(ApiResponse<OrderDetailsDto>.Success(result));
        }
        [HttpPost("pay")]
        public async Task<ActionResult<ApiResponse<PayOrderResultDto>>> PayMy(int id,int userId, PaymentMethod  paymentMethod)
        {
            var result = await _orderService.PayMyOrderAsync(id, userId, paymentMethod);
            return Ok(ApiResponse<PayOrderResultDto>.Success(result, "Order confirmed successfully"));
        }
        [HttpGet("getAll")]
        public async Task<ActionResult<ApiResponse<PagedResult<OrderListItemDto>>>> GetAllAsync([FromQuery] OrderSpecParams spec)
        {
            var result = await _orderService.GetAllAsync(spec);
            return Ok(ApiResponse<PagedResult<OrderListItemDto>>.Success(result));
        }
        [HttpGet("getWithId")]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> GetOrderWithId(int id )
        {
            var result = await _orderService.GetByIdAsync(id);

            return Ok(ApiResponse<OrderDetailsDto>.Success(result));
        }

        [HttpPost("cancel")]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> CancelMyOrder(int id, int userId)
        {
            var result = await _orderService.CancelMyOrderAsync(id, userId);
            return Ok(ApiResponse<OrderDetailsDto>.Success(result, "Order cancelled successfully"));
        }
    }
}
