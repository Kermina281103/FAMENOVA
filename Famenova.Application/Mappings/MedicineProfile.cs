using AutoMapper;
using famenova.Domain.Entities;
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
            CreateMap<CreateMedicineDto, Medicine>();

            CreateMap<Medicine, MedicineResponseDto>()
                .ForMember(
                    dest => dest.CategoryName,
                    opt => opt.MapFrom(x => x.Category.Name)
                );

            
        }
    }
}
