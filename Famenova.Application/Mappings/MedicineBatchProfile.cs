using AutoMapper;
using famenova.Domain.Entities;
using Famenova.Shared.Dtos.Medicine;
using Famenova.Shared.Dtos.MedicineBatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings
{
    public class MedicineBatchProfile:Profile
    {
        public MedicineBatchProfile()
        {
            CreateMap<CreateMedicineBatchDto, MedicineBatch>().ReverseMap();

            CreateMap<MedicineBatch, MedicineBatchResponseDto>()
                 .ForMember(dest => dest.MedicineName, sr => sr.MapFrom(x => x.Medicine.Name));
                
            


            
        }
    }
}
