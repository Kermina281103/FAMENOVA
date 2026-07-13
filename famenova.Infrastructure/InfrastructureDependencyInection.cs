using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using famenova.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using famenova.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace famenova.Infrastructure
{
    public static class InfrastructureDependencyInection
    {
      public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
       
      {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(connectionString));

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();



            return services;
        }
    }
}
