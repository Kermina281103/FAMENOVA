using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Dashboard
{


    public class PeriodNumbersDto
    {
        public decimal Revenue { get; set; }
        public int ConfirmedOrders { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class SalesPointDto
    {
        public string BucketStart { get; set; } = "";
        public decimal Revenue { get; set; }
        public int ConfirmedOrders { get; set; }
        public int TotalOrders { get; set; }
    }

    public class SalesAnalyticsDto : PeriodNumbersDto
    {
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public PeriodNumbersDto Previous { get; set; } = new();
        public List<SalesPointDto> Series { get; set; } = new();
    }

    public class RevenueKpisDto
    {
        public decimal Today { get; set; }
        public decimal Yesterday { get; set; }
        public decimal Last7Days { get; set; }
        public decimal Previous7Days { get; set; }
        public decimal MonthToDate { get; set; }
        public decimal PreviousMonthToDate { get; set; }
        public decimal AllTimeRevenue { get; set; }
        public int AllTimeOrders { get; set; }
    }

    public class StockLineDto
    {
        public int MedicineId { get; set; }
        public string Name { get; set; } = "";
        public int Stock { get; set; }
    }

    public class BatchLineDto
    {
        public int BatchId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = "";
        public string BatchNumber { get; set; } = "";
        public DateTime ExpiryDate { get; set; }
        public int Quantity { get; set; }
    }

    public class InventoryAnalyticsDto
    {
        public int TotalMedicines { get; set; }
        public int TotalUnits { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public List<StockLineDto> LowStock { get; set; } = new();
        public List<StockLineDto> OutOfStock { get; set; } = new();
        public List<StockLineDto> HighestStock { get; set; } = new();
        public List<StockLineDto> LowestStock { get; set; } = new();
        public List<BatchLineDto> ExpiringBatches { get; set; } = new();
        public List<BatchLineDto> ExpiredBatches { get; set; } = new();
    }

    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public bool IsMedicine { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopCategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = "";
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopProductsDto
    {
        public List<TopProductDto> Products { get; set; } = new();
        public List<TopCategoryDto> Categories { get; set; } = new();
    }
}

