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

    /// <summary>
    /// D5 — gross profit of confirmed orders in a period, from the cost of goods fixed at confirmation.
    /// Revenue = sum of item lines (delivery fees excluded). Lines without a known cost are NOT
    /// counted in the profit; they are reported so the UI can say the figure is partial.
    /// </summary>
    public class ProfitDto
    {
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public decimal Revenue { get; set; }
        /// <summary>Revenue of the lines whose cost is known (the base of the profit)</summary>
        public decimal CostedRevenue { get; set; }
        public decimal CostOfGoods { get; set; }
        /// <summary>CostedRevenue - CostOfGoods</summary>
        public decimal GrossProfit { get; set; }
        /// <summary>GrossProfit / CostedRevenue x 100; null when nothing is costed</summary>
        public decimal? MarginPercent { get; set; }
        public decimal UncostedRevenue { get; set; }
        public int UncostedLines { get; set; }
        /// <summary>CostedRevenue / Revenue x 100 (100 = every sale has a known cost)</summary>
        public decimal CoveragePercent { get; set; }
        /// <summary>Products sold without a known cost in this period (top 10 by revenue)</summary>
        public List<TopProductDto> MissingCostProducts { get; set; } = new();
        /// <summary>Batches still in stock with no unit cost (future sales won't be costed)</summary>
        public int BatchesWithoutCost { get; set; }
        /// <summary>Regular products (not medicines) with no purchase cost</summary>
        public int ProductsWithoutCost { get; set; }
    }

    public class TopProductsDto
    {
        public List<TopProductDto> Products { get; set; } = new();
        public List<TopCategoryDto> Categories { get; set; } = new();
    }
}

