using AutoMapper;
using famenova.Domain.Entities;
using Famenova.Application.Dtos.Category;
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
            CreateMap<CreateCategoryDto, Category>();
              

            CreateMap<UpdateCategoryDto, Category>();
                

            CreateMap<Category, CategoryResponseDto>();
        }
    }
}
