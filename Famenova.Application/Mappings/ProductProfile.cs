using AutoMapper;
using famenova.Domain.Entities;
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
            CreateMap<CreateProductDto, Product>();
            CreateMap<Product, ProductResponseDto>()
                .ForMember(x => x.CategoryName, sr => sr.MapFrom(x => x.Category.Name));
            CreateMap<UpdateProductDto, Product>();
        }
    }
}
