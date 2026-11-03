using AutoMapper;
using famenova.Domain.Entities;
using Famenova.Application.Dtos.Category;
using Famenova.Application.Mappings.ImageUrlResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest=>dest.ImageUrl,src=>src.Ignore());
              

            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest=>dest.ImageUrl,src=>src.Ignore());
                

            CreateMap<Category, CategoryResponseDto>()
                .ForMember(dest=>dest.ImageUrl,src=>src.MapFrom<CategoryImageResolver<CategoryResponseDto>>());
        }
    }
}
