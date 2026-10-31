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
        public async Task ConfirmAsync(Order order, PaymentMethod method)
        {
            if (!order.CanBePaid)
                throw new DomainException("Order can only be confirmed while pending and unpaid.");

            await EnsureStockAvailableAsync(order);
            var products = await LoadProductsAsync(order);

            foreach(var item in order.OrderItems)
            {
                var product = products[item.ProductId];
                product.DecreaseStock(item.Quantity);
            }
            if (method == PaymentMethod.CashOnDelivery)
            {
                order.ConfirmCashOnDelivery(); 
            }
            else if(method == PaymentMethod.Card)
            {
                order.MarkAsPaid();
            }

        }

        public async Task ReleaseStockAsync(Order order)
        {
            var products = await LoadProductsAsync(order);
                foreach(var item in order.OrderItems)
            {
                var product = products[item.ProductId];
                product.IncreaseStock(item.Quantity);
            }
        }

        public async Task<Dictionary<int,Product>> LoadProductsAsync(Order order)
        {
            var spec = new ProductByIdSpecification(order.OrderItems.Select(i => i.ProductId));

            var products = await _unitOfWork.GetGeneric<Product>().GetAllAsync(spec);
            return products.ToDictionary(p => p.Id); 
        }

        public async Task EnsureStockAvailableAsync(Order order)
        {
            var products = await LoadProductsAsync(order);

            foreach (var item in order.OrderItems)
            {
                var product = products[item.ProductId];
                if (product.Stock < item.Quantity)
                    throw new DomainException(
                        $"Insufficient stock for '{product.Name}'. Available: {product.Stock}, requested: {item.Quantity}.");
            }
        }
    }
}
