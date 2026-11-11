using Famenova.Application.Interfaces;
using Famenova.Application.Jwt;
using Famenova.Application.Otp;
using Famenova.Application.Services;
using Famenova.Application.Services.Files;
using Famenova.Application.Specifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EmailOtpSender = Famenova.Application.Services.EmailOtpSender;

namespace Famenova.Application
{
    public static  class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection service,IConfiguration configuration )
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
            service.AddScoped<IFileService, FileService>();
            service.AddScoped<IPaymentWebhookService, PaymentWebhookService>();
            service.Configure<JwtOptions>(
                 configuration.GetSection("Jwt"));
            service.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
            service.AddScoped<IAuthService, AuthService>();
            service.AddOptions<EmailSettings>()
        .Bind(configuration.GetSection(EmailSettings.SectionName))
        .ValidateDataAnnotations()
          .ValidateOnStart();

            service.AddScoped<IOtpSender, EmailOtpSender>();
            service.AddScoped<IOtpService, OtpService>();

            return service;
        }
    }
}
