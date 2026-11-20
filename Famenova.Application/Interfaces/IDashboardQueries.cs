using Famenova.Shared.Dtos.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface IDashboardQueries
    {
        Task<SalesAnalyticsDto> SalesAsync(DateOnly from, DateOnly to, string groupBy);
        Task<RevenueKpisDto> KpisAsync();
        Task<Dictionary<string, int>> OrderStatusAsync(DateOnly from, DateOnly to);
        Task<InventoryAnalyticsDto> InventoryAsync(int lowStock, int expiringDays, int take);
        Task<TopProductsDto> TopProductsAsync(DateOnly from, DateOnly to, int take);
    }
}
