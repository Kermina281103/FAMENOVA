using famenova.Domain.Constants;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Shared;
using Famenova.Shared.Dtos.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class DashboardController(IDashboardQueries queries) : ControllerBase
    {
        [HttpGet("sales")]
        public async Task<ActionResult<ApiResponse<SalesAnalyticsDto>>> Sales(
            DateOnly from, DateOnly to, string groupBy = "day")
        {
            CheckRange(from, to);
            groupBy = groupBy is "week" or "month" ? groupBy : "day";

            var result = await queries.SalesAsync(from, to, groupBy);
            return Ok(ApiResponse<SalesAnalyticsDto>.Success(result));
        }

        [HttpGet("kpis")]
        public async Task<ActionResult<ApiResponse<RevenueKpisDto>>> Kpis()
        {
            var result = await queries.KpisAsync();
            return Ok(ApiResponse<RevenueKpisDto>.Success(result));
        }

        [HttpGet("order-status")]
        public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> OrderStatus(
            DateOnly from, DateOnly to)
        {
            CheckRange(from, to);

            var result = await queries.OrderStatusAsync(from, to);
            return Ok(ApiResponse<Dictionary<string, int>>.Success(result));
        }

        [HttpGet("inventory")]
        public async Task<ActionResult<ApiResponse<InventoryAnalyticsDto>>> Inventory(
            int lowStock = 10, int expiringDays = 60, int take = 5)
        {
            var result = await queries.InventoryAsync(
                Math.Clamp(lowStock, 0, 1000),
                Math.Clamp(expiringDays, 1, 365),
                Math.Clamp(take, 1, 20));

            return Ok(ApiResponse<InventoryAnalyticsDto>.Success(result));
        }

        [HttpGet("top-products")]
        public async Task<ActionResult<ApiResponse<TopProductsDto>>> TopProducts(
            DateOnly from, DateOnly to, int take = 10)
        {
            CheckRange(from, to);

            var result = await queries.TopProductsAsync(from, to, Math.Clamp(take, 1, 50));
            return Ok(ApiResponse<TopProductsDto>.Success(result));
        }

        /// <summary>D5 — gross profit from the cost of the goods actually sold</summary>
        [HttpGet("profit")]
        public async Task<ActionResult<ApiResponse<ProfitDto>>> Profit(DateOnly from, DateOnly to)
        {
            CheckRange(from, to);
            var result = await queries.ProfitAsync(from, to);
            return Ok(ApiResponse<ProfitDto>.Success(result));
        }

        private static void CheckRange(DateOnly from, DateOnly to)
        {
            if (from == default || to == default)
                throw new BadRequestException("'from' and 'to' are required (format: yyyy-MM-dd).");

            if (from > to)
                throw new BadRequestException("'from' must be on or before 'to'.");

            if (to.DayNumber - from.DayNumber > 731)
                throw new BadRequestException("The range is limited to 2 years.");
        }
    }
}