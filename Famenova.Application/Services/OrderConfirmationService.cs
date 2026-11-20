using famenova.Domain.Entities;
using famenova.Domain.Enums;
using famenova.Domain.Exceptions;
using famenova.Domain.Interfaces;
using Famenova.Application.Interfaces;
using Famenova.Application.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Services
{
    public class OrderConfirmationService : IOrderConfirmationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderConfirmationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task EnsureStockAvailableAsync(Order order)
        {
            var products = await LoadProductsAsync(order);

            foreach (var item in order.OrderItems)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    throw new DomainException($"Product {item.ProductId} no longer exists.");

                if (product.Stock < item.Quantity)
                    throw new DomainException(
                        $"Insufficient stock for '{product.Name}'. Available: {product.Stock}, requested: {item.Quantity}.");
            }

            await PlanAllocationsAsync(order, products);  
        }

        public async Task ConfirmAsync(Order order, PaymentMethod method)
        {
            if (!order.CanBePaid)
                throw new DomainException("Order can only be confirmed while pending and unpaid.");

            await EnsureStockAvailableAsync(order);        // فحوصات بس
            var products = await LoadProductsAsync(order);

            await ApplyAllocationsAsync(order, products);  // الدفعات الأول
            foreach (var item in order.OrderItems)
                products[item.ProductId].DecreaseStock(item.Quantity);

            if (method == PaymentMethod.CashOnDelivery)
                order.ConfirmCashOnDelivery();
            else if (method == PaymentMethod.Card)
                order.MarkAsPaid();
        }

        private async Task<List<(OrderItem Item, MedicineBatch Batch, int Take)>> PlanAllocationsAsync(
            Order order, Dictionary<int, Product> products)
        {
            var plan = new List<(OrderItem, MedicineBatch, int)>();

            var medicineItems = order.OrderItems
                .Where(i => products.TryGetValue(i.ProductId, out var p) && p is Medicine)
                .ToList();
            if (medicineItems.Count == 0) return plan;

            var batches = (await _unitOfWork.GetGeneric<MedicineBatch>()
                    .GetAllAsync(new SellableBatchesSpecification(medicineItems.Select(i => i.ProductId))))
                .GroupBy(b => b.MedicineId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in medicineItems)
            {
                var left = item.Quantity;

                foreach (var batch in batches.GetValueOrDefault(item.ProductId) ?? new List<MedicineBatch>())
                {
                    if (left == 0) break;
                    var take = Math.Min(left, batch.Quantity);
                    plan.Add((item, batch, take));
                    left -= take;
                }

                if (left > 0)
                    throw new DomainException($"Not enough non-expired stock for '{products[item.ProductId].Name}'.");
            }

            return plan;
        }

        private async Task ApplyAllocationsAsync(Order order, Dictionary<int, Product> products)
        {
            foreach (var (item, batch, take) in await PlanAllocationsAsync(order, products))
            {
                batch.Consume(take);
                item.Allocations.Add(new OrderItemBatch { MedicineBatchId = batch.Id, Quantity = take });
            }
        }

        public async Task ReleaseStockAsync(Order order)
        {
            var products = await LoadProductsAsync(order);
            foreach (var item in order.OrderItems)
                products[item.ProductId].IncreaseStock(item.Quantity);

            var allocations = (await _unitOfWork.GetGeneric<OrderItemBatch>()
                .GetAllAsync(new AllocationsForOrderSpecification(order.OrderItems.Select(i => i.Id))))
                .ToList();

            foreach (var a in allocations)
            {
                a.MedicineBatch.Restore(a.Quantity);                       
                _unitOfWork.GetGeneric<OrderItemBatch>().Remove(a);        
            }
        }

        public async Task<Dictionary<int,Product>> LoadProductsAsync(Order order)
        {
            var spec = new ProductByIdSpecification(order.OrderItems.Select(i => i.ProductId));

            var products = await _unitOfWork.GetGeneric<Product>().GetAllAsync(spec);
            return products.ToDictionary(p => p.Id); 
        }

       
    }
}
