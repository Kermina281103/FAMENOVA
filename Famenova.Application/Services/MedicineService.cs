using AutoMapper;
using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using Famenova.Application.Common.Models;
using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Famenova.Application.Services.Files;
using Famenova.Application.Specifications;
using Famenova.Shared.Dtos.Medicine;

namespace Famenova.Application.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public MedicineService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<MedicineResponseDto> CreateAsync(CreateMedicineDto dto)
        {
            var findcat = await _unitOfWork.GetGeneric<Category>().GetByIdAsync(dto.CategoryId);
            if (findcat is null)
                throw new NotFoundException($"Category With Id {dto.CategoryId} is not Found");

            // 🆕
            if (dto.ProductGroupId.HasValue)
                await ValidateGroupAsync(dto.ProductGroupId.Value, dto.VariantLabel ?? dto.Concentration);

            var medicine = _mapper.Map<Medicine>(dto);
            if (dto.ImageUrl != null)
                medicine.ImageUrl = await _fileService.SaveImageAsync(dto.ImageUrl, "Medicines");

            medicine.ProductGroupId = dto.ProductGroupId;
            medicine.VariantLabel = dto.VariantLabel;

            await _unitOfWork.GetGeneric<Medicine>().AddAsync(medicine);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(medicine.Id);
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
            var medicineRepo = _unitOfWork.GetGeneric<Medicine>();
            var medicine = await medicineRepo.GetByIdAsync(new MedicineSpecification(id));
            if (medicine is null)
                throw new NotFoundException($"Medicine With Id : {id} is not found");

            var response = _mapper.Map<MedicineResponseDto>(medicine);

            // 🆕 التركيزات/الأحجام
            var rootId = medicine.ProductGroupId ?? medicine.Id;
            var variants = await medicineRepo.ListAsync(MedicineSpecification.Variants(rootId));

            if (variants.Count > 1)
                response.Variants = _mapper.Map<List<MedicineVariantDto>>(variants);

            return response;
        }

        public async Task<MedicineResponseDto> UpdateAsync(int medicineId, UpdateMedicineDto dto)
        {
            var medRepo = _unitOfWork.GetGeneric<Medicine>();
            var medicine = await medRepo.GetByIdAsync(new MedicineSpecification(medicineId));
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
                medicine.RequiresPrescription = dto.RequiresPrescription.Value;

            // 🆕 اسم الحجم/التركيز
            if (!string.IsNullOrEmpty(dto.VariantLabel))
                medicine.VariantLabel = dto.VariantLabel;

            // 🆕 المجموعة (0 = شيليه من المجموعة)
            if (dto.ProductGroupId.HasValue)
            {
                if (dto.ProductGroupId.Value == 0)
                {
                    medicine.ProductGroupId = null;
                }
                else
                {
                    await ValidateGroupAsync(dto.ProductGroupId.Value, medicine.VariantLabel ?? medicine.Concentration, medicineId);
                    medicine.ProductGroupId = dto.ProductGroupId.Value;
                }
            }

            if (dto.ImageUrl != null)
            {
                if (medicine.ImageUrl != null)
                    _fileService.DeleteImage(medicine.ImageUrl);
                medicine.ImageUrl = await _fileService.SaveImageAsync(dto.ImageUrl, "Medicines");
            }
            #endregion

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<MedicineResponseDto>(medicine);
        }

        private async Task ValidateGroupAsync(int groupId, string? label, int? currentId = null)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new BadRequestException("Variant label or concentration is required when the medicine belongs to a group");

            if (currentId == groupId)
                throw new BadRequestException("A medicine can't be grouped under itself");

            // GetGeneric<Medicine> مش هيلاقي المنتج لو كان منتج عادي مش دوا
            var root = await _unitOfWork.GetGeneric<Medicine>().GetByIdAsync(groupId)
                ?? throw new NotFoundException($"There isn't medicine with Id : {groupId}");

            if (root.IsDeleted)
                throw new BadRequestException("The group's main medicine is deleted");

            if (root.ProductGroupId.HasValue)
                throw new BadRequestException($"Medicine {groupId} is a variant itself. Use its main medicine Id : {root.ProductGroupId}");
        }
    }
}