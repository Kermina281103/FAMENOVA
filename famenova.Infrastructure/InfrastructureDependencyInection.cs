
using famenova.Domain.Entities;
using famenova.Domain.Interfaces;

using famenova.Infrastructure.Carts;
using famenova.Infrastructure.Data.Context;
using famenova.Infrastructure.Payments;
using famenova.Infrastructure.Persistence.Repositories;
using famenova.Infrastructure.Persistence.UnitOfworks;
using famenova.Infrastructure.Jwt;

using Famenova.Application.Interfaces;
using Famenova.Application.Jwt;
using Famenova.Application.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

namespace famenova.Infrastructure
{
    public static class InfrastructureDependencyInection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
      

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDataProtection();


          

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                // User Settings
                options.User.RequireUniqueEmail = true;

                // Password Settings
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                // Lockout Settings
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


       

            services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
            {
                var redisConnection =
                    configuration.GetConnectionString("Redis");

                if (string.IsNullOrWhiteSpace(redisConnection))
                {
                    throw new InvalidOperationException(
                        "Redis connection string is missing.");
                }

                return ConnectionMultiplexer.Connect(redisConnection);
            });



            services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));

            services.AddScoped<IPrescriptionOrderRepository,
                PrescriptionOrderRepository>();

            services.AddScoped<IMedicineRepository,
                MedicineRepository>();

            services.AddScoped<ICartRepository,
                CartRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();


            // =========================================
            // 5. Payment Gateway Configuration
            // =========================================

            services.Configure<FakeGatewayOptions>(
                configuration.GetSection("FakeGateway"));

            services.Configure<PaymobOptions>(
                configuration.GetSection("Paymob"));

            var gateway = configuration["Payment:Gateway"];

            if (gateway == "Paymob")
            {
                services.AddHttpClient<PaymobPaymentGateway>(client =>
                {
                    client.BaseAddress =
                        new Uri("https://accept.paymob.com/");
                });

                services.AddScoped<IPaymentGateWay>(
                    provider =>
                        provider.GetRequiredService<PaymobPaymentGateway>());
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unknown payment gateway configuration: {gateway}");
            }


           

            services.Configure<JwtOptions>(
                configuration.GetSection(JwtOptions.SectionName));

            services.AddScoped<IJwtTokenGenerator,
                JwtTokenGenerator>();



            services.AddScoped<IAuthService, AuthService>();


            return services;
        }
    }
}
