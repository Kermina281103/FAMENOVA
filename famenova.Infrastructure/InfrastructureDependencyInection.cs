

using famenova.Domain.Interfaces;
using famenova.Infrastructure.Carts;
using famenova.Infrastructure.Data.Context;
using famenova.Infrastructure.Payments;
using famenova.Infrastructure.Persistence.Repositories;
using famenova.Infrastructure.Persistence.UnitOfworks;
using Famenova.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

using famenova.Domain.Entities;


namespace famenova.Infrastructure
{
    public static class InfrastructureDependencyInection
    {
      public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
       
      {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(connectionString));
            services.AddDataProtection();
            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddSingleton(typeof(IConnectionMultiplexer), (serviceProvider) =>
            {
                var connection = configuration.GetConnectionString("Redis");
                var connectionMultiplexerobj = ConnectionMultiplexer.Connect(connection!);
                return connectionMultiplexerobj;
            });
 
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IPrescriptionOrderRepository, PrescriptionOrderRepository>();
            services.AddScoped<IMedicineRepository, MedicineRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.Configure<FakeGatewayOptions>(configuration.GetSection("FakeGateway"));
           
            services.Configure<PaymobOptions>(
                 configuration.GetSection("Paymob"));
            var gateway = configuration["Payment:Gateway"];
            if (gateway == "Paymob")
            {
                services.AddHttpClient<PaymobPaymentGateway>(x =>
                x.BaseAddress = new Uri("https://accept.paymob.com/"));

                services.AddScoped<IPaymentGateWay>(
                    s => s.GetRequiredService<PaymobPaymentGateway>());
            }
            else
            {
                throw new InvalidOperationException("Unknown payment gateway configuration");
            }

                return services;
        }
    }
}
