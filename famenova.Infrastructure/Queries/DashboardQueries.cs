using Famenova.Application.Interfaces;
using famenova.Domain.Enums;
using famenova.Infrastructure.Data.Context;
using Famenova.Shared.Dtos.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace famenova.Infrastructure.Queries
{
    public class DashboardQueries(AppDbContext db) : IDashboardQueries
    {
        private static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");


        private static readonly OrderStatus[] Confirmed =
            { OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered };

        private record Row(DateTime OrderDate, decimal Total, OrderStatus Status);

        // ---------- helpers: Cairo calendar days <-> UTC ----------

        private static DateTime Utc(DateOnly day)
        {
            var local = day.ToDateTime(TimeOnly.MinValue);
            if (Cairo.IsInvalidTime(local)) local = local.AddHours(1);   // clock-change day
            return TimeZoneInfo.ConvertTimeToUtc(local, Cairo);
        }

        private static DateOnly Day(DateTime utc) =>
            DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.SpecifyKind(utc, DateTimeKind.Utc), Cairo));

        private static DateOnly Today => Day(DateTime.UtcNow);

        private Task<List<Row>> Rows(DateOnly from, DateOnly to)
        {
            DateTime a = Utc(from), b = Utc(to.AddDays(1));
            return db.Order.AsNoTracking()
                .Where(o => o.OrderDate >= a && o.OrderDate < b)
                .Select(o => new Row(o.OrderDate, o.Total, o.OrderStatus))
                .ToListAsync();
        }

        private static PeriodNumbersDto Numbers(ICollection<Row> rows)
        {
            var confirmed = rows.Where(r => Confirmed.Contains(r.Status)).ToList();
            var revenue = confirmed.Sum(r => r.Total);
            return new()
            {
                Revenue = revenue,
                ConfirmedOrders = confirmed.Count,
                TotalOrders = rows.Count,
                AverageOrderValue = confirmed.Count == 0 ? 0 : Math.Round(revenue / confirmed.Count, 2)
            };
        }

        private static DateOnly Bucket(DateOnly d, string groupBy) => groupBy switch
        {
            "week" => d.AddDays(-(((int)d.DayOfWeek + 1) % 7)),
            "month" => new DateOnly(d.Year, d.Month, 1),
            _ => d
        };

        private static DateOnly Next(DateOnly d, string groupBy) => groupBy switch
        {
            "week" => d.AddDays(7),
            "month" => d.AddMonths(1),
            _ => d.AddDays(1)
        };


        public async Task<SalesAnalyticsDto> SalesAsync(DateOnly from, DateOnly to, string groupBy)
        {
            var length = to.DayNumber - from.DayNumber + 1;

            var all = await Rows(from.AddDays(-length), to);
            var current = all.Where(r => Day(r.OrderDate) >= from).ToList();
            var previous = all.Where(r => Day(r.OrderDate) < from).ToList();

            var byBucket = current
                .GroupBy(r => Bucket(Day(r.OrderDate), groupBy))
                .ToDictionary(g => g.Key, g => g.ToList());

            var series = new List<SalesPointDto>();
            for (var bucket = Bucket(from, groupBy); bucket <= to; bucket = Next(bucket, groupBy))
            {
                var n = Numbers(byBucket.GetValueOrDefault(bucket) ?? new List<Row>());
                series.Add(new()
                {
                    BucketStart = bucket.ToString("yyyy-MM-dd"),
                    Revenue = n.Revenue,
                    ConfirmedOrders = n.ConfirmedOrders,
                    TotalOrders = n.TotalOrders
                });
            }

            var now = Numbers(current);
            return new()
            {
                From = from.ToString("yyyy-MM-dd"),
                To = to.ToString("yyyy-MM-dd"),
                Revenue = now.Revenue,
                ConfirmedOrders = now.ConfirmedOrders,
                TotalOrders = now.TotalOrders,
                AverageOrderValue = now.AverageOrderValue,
                Previous = Numbers(previous),
                Series = series
            };
        }


        public async Task<RevenueKpisDto> KpisAsync()
        {
            var t = Today;
            var monthStart = new DateOnly(t.Year, t.Month, 1);
            var prevMonthStart = monthStart.AddMonths(-1);

            var prevCut = prevMonthStart.AddDays(t.Day - 1);
            if (prevCut >= monthStart) prevCut = monthStart.AddDays(-1);

            var earliest = prevMonthStart < t.AddDays(-13) ? prevMonthStart : t.AddDays(-13);
            var rows = await Rows(earliest, t);

            decimal Rev(DateOnly a, DateOnly b) => rows
                .Where(r => Confirmed.Contains(r.Status) && Day(r.OrderDate) >= a && Day(r.OrderDate) <= b)
                .Sum(r => r.Total);

            return new()
            {
                Today = Rev(t, t),
                Yesterday = Rev(t.AddDays(-1), t.AddDays(-1)),
                Last7Days = Rev(t.AddDays(-6), t),
                Previous7Days = Rev(t.AddDays(-13), t.AddDays(-7)),
                MonthToDate = Rev(monthStart, t),
                PreviousMonthToDate = Rev(prevMonthStart, prevCut),
                AllTimeRevenue = await db.Order.AsNoTracking()
                    .Where(o => Confirmed.Contains(o.OrderStatus)).SumAsync(o => o.Total),
                AllTimeOrders = await db.Order.CountAsync()
            };
        }


        public async Task<Dictionary<string, int>> OrderStatusAsync(DateOnly from, DateOnly to)
        {
            var rows = await Rows(from, to);
            return Enum.GetValues<OrderStatus>()
                .ToDictionary(s => s.ToString(), s => rows.Count(r => r.Status == s));
        }


        public async Task<InventoryAnalyticsDto> InventoryAsync(int lowStock, int expiringDays, int take)
        {
            var meds = await db.Medicine.AsNoTracking()
                .Where(m => !m.IsDeleted)
                .Select(m => new StockLineDto { MedicineId = m.Id, Name = m.Name, Stock = m.Stock })
                .ToListAsync();

            var inStock = meds.Where(m => m.Stock > 0).ToList();
            var low = inStock.Where(m => m.Stock <= lowStock).OrderBy(m => m.Stock).ToList();
            var outOf = meds.Where(m => m.Stock <= 0).OrderBy(m => m.Name).ToList();

            var today = DateTime.UtcNow.Date;
            var limit = today.AddDays(expiringDays);

            var batches = await db.Batches.AsNoTracking()
                .Where(b => b.Quantity > 0 && b.ExpiryDate <= limit && !b.Medicine.IsDeleted)
                .OrderBy(b => b.ExpiryDate)
                .Select(b => new BatchLineDto
                {
                    BatchId = b.Id,
                    MedicineId = b.MedicineId,
                    MedicineName = b.Medicine.Name,
                    BatchNumber = b.BatchNumber,
                    ExpiryDate = b.ExpiryDate,
                    Quantity = b.Quantity
                })
                .ToListAsync();

            return new()
            {
                TotalMedicines = meds.Count,
                TotalUnits = inStock.Sum(m => m.Stock),
                LowStockCount = low.Count,
                OutOfStockCount = outOf.Count,
                LowStock = low,
                OutOfStock = outOf,
                HighestStock = inStock.OrderByDescending(m => m.Stock).Take(take).ToList(),
                LowestStock = inStock.OrderBy(m => m.Stock).Take(take).ToList(),
                ExpiredBatches = batches.Where(b => b.ExpiryDate.Date <= today).ToList(),
                ExpiringBatches = batches.Where(b => b.ExpiryDate.Date > today).ToList()
            };
        }


        public async Task<TopProductsDto> TopProductsAsync(DateOnly from, DateOnly to, int take)
        {
            DateTime a = Utc(from), b = Utc(to.AddDays(1));

            var items = db.OrderItem.AsNoTracking()
                .Where(i => Confirmed.Contains(i.Order.OrderStatus)
                         && i.Order.OrderDate >= a && i.Order.OrderDate < b);

            var products = await items
                .GroupBy(i => new { i.ProductId, i.Product!.Name })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    Name = g.Key.Name,
                    UnitsSold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderByDescending(x => x.UnitsSold)
                .Take(take)
                .ToListAsync();

            var ids = products.Select(p => p.ProductId).ToList();
            var medicineIds = await db.Medicine.Where(m => ids.Contains(m.Id)).Select(m => m.Id).ToListAsync();
            products.ForEach(p => p.IsMedicine = medicineIds.Contains(p.ProductId));

            var categories = await items
                .GroupBy(i => new { i.Product!.CategoryId, i.Product.Category.Name })
                .Select(g => new TopCategoryDto
                {
                    CategoryId = g.Key.CategoryId,
                    Name = g.Key.Name,
                    UnitsSold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(take)
                .ToListAsync();

            return new() { Products = products, Categories = categories };
        }
    }
}
