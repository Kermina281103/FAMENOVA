using AutoMapper;
using famenova.Domain.Entities;
using Famenova.Shared.Dtos.PrescriptionOrder;
using Microsoft.Extensions.Configuration;

namespace Famenova.Application.Mappings.ImageUrlResolver
{
    public class MessageAttachmentResolver(IConfiguration config)
        : IValueResolver<PrescriptionMessage, PrescriptionMessageDto, string?>
    {
        public string? Resolve(PrescriptionMessage s, PrescriptionMessageDto d, string? m, ResolutionContext c) =>
            string.IsNullOrEmpty(s.AttachmentUrl) ? null : $"{config["Urls:apiBaseUrl"]}{s.AttachmentUrl}";
    }
}
