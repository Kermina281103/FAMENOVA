using AutoMapper;
using AutoMapper.Execution;
using famenova.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings.ImageUrlResolver
{
    class CategoryImageResolver<T>(IConfiguration config) : IValueResolver<Category, T, string>
    {
        public string Resolve(Category source, T destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImageUrl))
            {
                return $"{config["Urls:apiBaseUrl"]}{source.ImageUrl}";
            }
            return string.Empty;
        }

    }
}
