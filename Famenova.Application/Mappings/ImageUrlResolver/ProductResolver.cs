using AutoMapper;
using famenova.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings.ImageUrlResolver
{
    public class ProductResolver<T> : IValueResolver<Product, T, string>
    {
        private readonly IConfiguration _config;
        public ProductResolver(IConfiguration config)
        {
            _config = config;
        }
        public string Resolve(Product source, T destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImageUrl))
            {
                return $"{_config["Urls:apiBaseUrl"]}{source.ImageUrl}";
            }
            return string.Empty;
           
        }
    }
}
