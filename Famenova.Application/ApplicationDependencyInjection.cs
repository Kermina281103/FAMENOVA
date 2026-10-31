using Famenova.Application.Interfaces;
using Famenova.Application.Services;
using Famenova.Application.Specifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application
{
    public static  class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection service )
        {
            service.AddAutoMapper(_=> { },typeof(ApplicationDependencyInjection).Assembly);

            service.AddScoped<ICategoryService, CategoryService>();
            service.AddScoped<IMedicineService, MedicineService>();
            service.AddScoped<IMedicineBatchService, MedicineBatchService>();
            service.AddScoped<IPrescriptionOrderService, PrescriptionOrderService>();
            service.AddScoped<ICartService, CartService>();
            service.AddScoped<IOrderService, OrderService>();
            service.AddScoped<IProductService, ProductService>();
            service.AddScoped<IOrderConfirmationService, OrderConfirmationService>();
            service.AddScoped<IPaymentWebhookService, PaymentWebhookService>();
            return service;
        }
    }
}
