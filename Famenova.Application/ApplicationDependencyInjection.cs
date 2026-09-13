using Famenova.Application.Interfaces;
using Famenova.Application.Services;
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
            return service;
        }
    }
}
