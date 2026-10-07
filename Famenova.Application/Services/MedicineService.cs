using AutoMapper;
using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using Famenova.Application.Common.Models;
using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Famenova.Application.Specifications;
using Famenova.Shared.Dtos.Medicine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Services
{
    public class MedicineService : IMedicineService

    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MedicineService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<MedicineResponseDto> CreateAsync(CreateMedicineDto dto)
        {
            var findcat = await _unitOfWork.GetGeneric<Category>().GetByIdAsync(dto.CategoryId);
            if (findcat is null)
                throw new NotFoundException($"Category With Id {dto.CategoryId} is not Found");
            var medicine =  _mapper.Map<Medicine>(dto);
          await   _unitOfWork.GetGeneric<Medicine>().AddAsync(medicine);
           await  _unitOfWork.SaveChangesAsync();
            var medicineResponse=  _mapper.Map<MedicineResponseDto>(medicine);
            
            return medicineResponse;
                                      
        }

        public async Task DeleteAsync(int id)
        {
            var medRepo = _unitOfWork.GetGeneric<Medicine>();
            var medicine = await medRepo.GetByIdAsync(id);
            if (medicine is null)
                throw new NotFoundException($"Medicine With Id : {id} is not found ");
            medRepo.Remove(medicine);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<MedicineResponseDto>> GetAllAsync(string? search, int? categoryId, bool? requiresPrescription, string? sort, int pageIndex, int pageSize)
        {
            var spec = new MedicineSpecification(search, categoryId, requiresPrescription, sort, pageIndex, pageSize);
            var result = await _unitOfWork.GetGeneric<Medicine>().GetPagedAsync(spec);

            var items = _mapper.Map<IEnumerable<MedicineResponseDto>>(result.Items);

            return new PagedResult<MedicineResponseDto>(items, result.PageNumber, result.PageSize, result.TotalCount);


        }

        public async Task<MedicineResponseDto> GetByIdAsync(int id)
        {
            var medicinceRepo = _unitOfWork.GetGeneric<Medicine>();
            var spec = new MedicineSpecification(id);
            var medicine = await medicinceRepo.GetByIdAsync(spec);
            if (medicine is null)
                throw new NotFoundException($"Medicine With Id : {id} is not found");
            return _mapper.Map<MedicineResponseDto>(medicine);

        }

        public async Task<MedicineResponseDto> UpdateAsync(int medicineId, UpdateMedicineDto dto)
        {
            var medRepo = _unitOfWork.GetGeneric<Medicine>();
            var spec = new MedicineSpecification(medicineId);
            var medicine = await medRepo.GetByIdAsync(spec);
            if (medicine is null)
                throw new NotFoundException($"Medicine With Id {medicineId} is not found ");

            #region mapping update Manually 

            if (!string.IsNullOrEmpty(dto.Name))
                medicine.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Concentration))
                medicine.Concentration = dto.Concentration;
            if (!string.IsNullOrEmpty(dto.ActiveIngredient))
                medicine.ActiveIngredient = dto.ActiveIngredient;
            if (!string.IsNullOrEmpty(dto.Description))
                medicine.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.DosageForm))
                medicine.DosageForm = dto.DosageForm;
            if (!string.IsNullOrEmpty(dto.ImageUrl))
                medicine.ImageUrl = dto.ImageUrl;
            if (dto.CategoryId.HasValue)
            {
                var cat = await _unitOfWork.GetGeneric<Category>().GetByIdAsync(dto.CategoryId.Value);
                if (cat is null)
                    throw new NotFoundException($"Category With id {dto.CategoryId.Value} is not found ");
                medicine.CategoryId = dto.CategoryId.Value;

            }
            if (dto.Price.HasValue)
                medicine.Price = dto.Price.Value;
            if (dto.RequiresPrescription.HasValue)
            {
                medicine.RequiresPrescription = dto.RequiresPrescription.Value;
            }

            #endregion

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MedicineResponseDto>(medicine);
        }
    }
}
