using AutoMapper;
using famenova.Domain.Entities;
using Famenova.Application.Mappings.ImageUrlResolver;
using Famenova.Shared.Dtos.Medicine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings
{
    public class MedicineProfile:Profile
    {
        public MedicineProfile()
        {
            CreateMap<CreateMedicineDto, Medicine>()
                .ForMember(ds => ds.ImageUrl, src => src.Ignore());

            CreateMap<Medicine, MedicineResponseDto>()
                .ForMember(
                    dest => dest.CategoryName,
                    opt => opt.MapFrom(x => x.Category.Name)
                )
                .ForMember(dest=>dest.ImageUrl,src=>src.MapFrom<MedicineImageResolver<MedicineResponseDto>>())
                .ForMember(d => d.Variants, o => o.Ignore());

            CreateMap<Medicine, MedicineVariantDto>()
                .ForMember(d => d.VariantLabel, o => o.MapFrom(s => s.VariantLabel ?? s.Concentration ?? s.Name));


        }
    }
}
