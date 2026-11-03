using AutoMapper;
using AutoMapper.Execution;
using famenova.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings.ImageUrlResolver
{
    public class MedicineImageResolver<T>(IConfiguration config) : IValueResolver<Medicine, T, string>
    {
        public string Resolve(Medicine source, T destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImageUrl))
            {
                  return $"{config["Urls:apiBaseUrl"]}{source.ImageUrl}";
            }
            return string.Empty;
        }
    }
}
