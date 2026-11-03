using AutoMapper;
using famenova.Domain.Common;
using famenova.Domain.Entities;
using Famenova.Application.Mappings.ImageUrlResolver;
using Famenova.Shared.Dtos.Common;
using Famenova.Shared.Dtos.Order;
using Famenova.Shared.Dtos.PrescriptionOrder;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings
{
    public class PrescriptionOrderProfile:Profile
    {
        public PrescriptionOrderProfile()
        {
            CreateMap<Address, AddressDto>().ReverseMap();
            CreateMap<PrescriptionOrder, PrescriptionOrderListItemDto>()
                   .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                   .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn))
                   .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.UserName))
                   .ForMember(dest => dest.PrescriptionImageUrl, opt => opt.MapFrom<PrescriptionImageResolver<PrescriptionOrderListItemDto>>());
            CreateMap<PrescriptionOrderItem, PrescriptionOrderItemDto>()
                    .ForMember(dest => dest.ProductName,
                         opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<CreatePrescriptionOrderDto, PrescriptionOrder>()
               .ForMember(dest => dest.PrescriptionImageUrl, src => src.Ignore());


            CreateMap<PrescriptionOrder, PrescriptionOrderResponseDto>()
                .ForMember(de => de.Status, src => src.MapFrom(x => x.Status))
                .ForMember(desc => desc.PrescriptionImageUrl, src => src.MapFrom<PrescriptionImageResolver<PrescriptionOrderResponseDto>>());
            
          //  CreateMap<PrescriptionOrder, OrderResponseDto>()
          //      .ForMember(dest => dest.OrderStatus, sr => sr.MapFrom(x => x.Order!.OrderStatus))
          //      .ForMember(dest => dest.PaymentStatus, sr => sr.MapFrom(x => x.Order!.PaymentStatus));
            CreateMap<PrescriptionOrder, PrescriptionOrderListItemDto>()
                .ForMember(dest => dest.CustomerName, sr => sr.MapFrom(x => x.Customer.UserName))
                .ForMember(dest => dest.Status, sr => sr.MapFrom(x => x.Status.ToString()))
          .ForMember(dest => dest.PrescriptionImageUrl, opt => opt.MapFrom<PrescriptionImageResolver<PrescriptionOrderListItemDto>>());

            CreateMap<PrescriptionOrder, PrescriptionOrderDetailsDto>()
               .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.UserName))
               .ForMember(dest => dest.Status,opt => opt.MapFrom(src => src.Status.ToString()))
               .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(desc => desc.PrescriptionImageUrl, src => src.MapFrom<PrescriptionImageResolver<PrescriptionOrderDetailsDto>>());


        }
    }
}
