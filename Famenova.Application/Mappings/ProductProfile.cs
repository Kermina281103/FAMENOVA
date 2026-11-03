using AutoMapper;
using famenova.Domain.Entities;
using Famenova.Application.Mappings.ImageUrlResolver;
using Famenova.Shared.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {

            CreateMap<CreateProductDto, Product>()
           .ForMember(x => x.ImageUrl, opt => opt.Ignore());
            
            CreateMap<Product, ProductVariantDto>()
    .ForMember(d => d.VariantLabel, o => o.MapFrom(s => s.VariantLabel ?? s.Name));
            CreateMap<Product, ProductResponseDto>()
            .ForMember(x => x.CategoryName, sr => sr.MapFrom(x => x.Category.Name))
            .ForMember(x => x.ImageUrl, src => src.MapFrom<ProductResolver<ProductResponseDto>>());
            
        }
    }
}
