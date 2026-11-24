using famenova.Domain.Entities;
using Famenova.Shared;

namespace Famenova.Application.Common.Costs
{
    /// <summary>Purchase cost rules (D5): validation and the cost of goods of a sold line</summary>
    internal static class CostRules
    {
        public const decimal MaxCost = 1_000_000m;

        /// <summary>null stays null (= not recorded); otherwise 0..MaxCost, rounded to 2 decimals</summary>
        public static decimal? Validate(decimal? cost)
        {
            if (cost is null) return null;
            if (cost < 0)
                throw new BadRequestException("Purchase cost can't be negative.");
            if (cost > MaxCost)
                throw new BadRequestException("Purchase cost is too large.");
            return Math.Round(cost.Value, 2);
        }

        /// <summary>
        /// Cost of goods of an order line, or null when any part of it has no recorded cost
        /// (a partly known cost would understate the cost and overstate the profit).
        /// Medicines: the batches actually consumed (allocations). Other products: PurchaseCost x quantity.
        /// </summary>
        public static decimal? LineCost(OrderItem item, Product product)
        {
            if (product is Medicine)
                return AllocatedCost(item);

            return product.PurchaseCost is decimal unit
                ? Math.Round(unit * item.Quantity, 2)
                : null;
        }

        /// <summary>Same as LineCost for a medicine line, from its stored allocations</summary>
        public static decimal? AllocatedCost(OrderItem item)
        {
            var allocations = item.Allocations.ToList();
            if (allocations.Count == 0 || allocations.Sum(a => a.Quantity) != item.Quantity)
                return null;
            if (allocations.Any(a => a.UnitCost is null))
                return null;
            return Math.Round(allocations.Sum(a => a.Quantity * a.UnitCost!.Value), 2);
        }
    }
}
