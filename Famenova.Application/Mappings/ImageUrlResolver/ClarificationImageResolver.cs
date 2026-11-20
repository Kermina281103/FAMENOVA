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
    public class ClarificationImageResolver<T>(IConfiguration config)
    : IValueResolver<PrescriptionOrder, T, string?>
    {
        public string? Resolve(PrescriptionOrder s, T d, string? m, ResolutionContext c) =>
            string.IsNullOrEmpty(s.ClarificationImageUrl)
                ? null
                : $"{config["Urls:apiBaseUrl"]}{s.ClarificationImageUrl}";
    }
}
