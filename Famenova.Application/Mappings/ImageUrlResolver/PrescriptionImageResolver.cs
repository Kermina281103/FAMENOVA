using AutoMapper;
using famenova.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings.ImageUrlResolver
{
    public class PrescriptionImageResolver<TEntity>(IConfiguration config) : IValueResolver<PrescriptionOrder, TEntity, string>
    {
        public string Resolve(PrescriptionOrder source, TEntity destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PrescriptionImageUrl))
            {
                return $"{config["Urls:apiBaseUrl"]}{source.PrescriptionImageUrl}";

            }
            return string.Empty;
        }
    }
}
