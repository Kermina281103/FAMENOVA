using AutoMapper;
using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using Famenova.Application.Common.Batches;
using Famenova.Application.Common.Models;
using Famenova.Application.Interfaces;
using Famenova.Application.Specifications;
using Famenova.Shared;
using Famenova.Shared.Dtos.MedicineBatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Services
{
    public class MedicineBatchService(IUnitOfWork _unitOfWork,IMapper _mapper) : IMedicineBatchService
    {
        public async Task<MedicineBatchResponseDto> CreateAsync(
      CreateMedicineBatchDto dto)
        {
            dto.BatchNumber = BatchRules.ValidateNumber(dto.BatchNumber);

            dto.ExpiryDate = BatchRules.ValidateNewExpiry(dto.ExpiryDate);

            BatchRules.ValidateQuantity(dto.Quantity, isNew: true);

            var medicine = await _unitOfWork
                .GetGeneric<Medicine>()
                .GetByIdAsync(dto.MedicineId);

            if (medicine is null)
                throw new NotFoundException(
                    $"Medicine With Id {dto.MedicineId} is not found");

            var exists = await _unitOfWork
                .GetGeneric<MedicineBatch>()
                .AnyAsync(x =>
                    x.MedicineId == dto.MedicineId &&
                    x.BatchNumber == dto.BatchNumber);

            if (exists)
                throw new ConflictException(
                    $"Batch Number {dto.BatchNumber} already exists for this medicine");

            var result = _mapper.Map<MedicineBatch>(dto);

            //  Increase medicine stock
            medicine.IncreaseStock(dto.Quantity);

          // Add batch to database
            await _unitOfWork
                .GetGeneric<MedicineBatch>()
                .AddAsync(result);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MedicineBatchResponseDto>(result);
        }

        public async Task DeleteAsync(int id)
        {
            if (await _unitOfWork.GetGeneric<OrderItemBatch>().AnyAsync(a => a.MedicineBatchId == id))
                throw new BadRequestException("This batch has sales history. Set its quantity to 0 instead.");
            var spec = new MedicineBatchSpecification(id);
            var medBatch =  await _unitOfWork.GetGeneric<MedicineBatch>().GetByIdAsync(spec);
            
            if (medBatch is null)
                throw new NotFoundException($"Medicine Batch With Id {id} is not found");
            medBatch.Medicine.DecreaseStock(medBatch.Quantity);
             _unitOfWork.GetGeneric<MedicineBatch>().Remove(medBatch);
            await _unitOfWork.SaveChangesAsync();
            
        }

        public async Task<PagedResult<MedicineBatchResponseDto>> GetAllAsync(string? search, int? medicineId,string? sort, int pageIndex, int pageSize)
        {
            var medBatchSpecification =  new MedicineBatchSpecification(search, medicineId,sort, pageIndex, pageSize);
            var result = await _unitOfWork.GetGeneric<MedicineBatch>().GetPagedAsync(medBatchSpecification);

            var items= _mapper.Map<IEnumerable<MedicineBatchResponseDto>>(result.Items);
            return new PagedResult<MedicineBatchResponseDto>(items, result.PageNumber, result.PageSize, result.TotalCount);

        }

        public async Task<MedicineBatchResponseDto> GetByIdAsync(int id)
        {
            var medBatchSpec = new MedicineBatchSpecification(id);
            var result = await _unitOfWork.GetGeneric<MedicineBatch>().GetByIdAsync(medBatchSpec);

            if (result is null)
                throw new NotFoundException($"Medicine Batch With Id {id} is not found ");

            return _mapper.Map<MedicineBatchResponseDto>(result);
        }

        public async Task<MedicineBatchResponseDto> UpdateAsync(int batchId,UpdateMedicineBatchDto dto)
        {
            var spec = new MedicineBatchSpecification(batchId);
            var medBatch = await _unitOfWork.GetGeneric<MedicineBatch>().GetByIdAsync(spec);
            if (medBatch is null)
                throw new NotFoundException($"medBatch With Id {batchId} is not found");


            if (!string.IsNullOrEmpty(dto.BatchNumber))
            {

                var exists = await _unitOfWork.GetGeneric<MedicineBatch>().AnyAsync(
                   x => x.MedicineId == medBatch.MedicineId
                  && x.BatchNumber == dto.BatchNumber
                  && x.Id != batchId);

                if (exists)
                    throw new ConflictException(
                        $"Batch Number {dto.BatchNumber} already exists for this medicine");

                medBatch.BatchNumber =
                         BatchRules.ValidateNumber(dto.BatchNumber);
            }

            if (dto.ExpiryDate.HasValue)
                medBatch.ExpiryDate = dto.ExpiryDate.Value.Date;

            if (dto.Quantity.HasValue)
            {
                var oldQuantity = medBatch.Quantity;
                var newQuantity = dto.Quantity.Value;

                var difference = newQuantity - oldQuantity;
                BatchRules.ValidateQuantity(
                   dto.Quantity.Value,
                    isNew: false);

                medBatch.Quantity = newQuantity;

                medBatch.Medicine.AdjustStock(difference);

  

              

            }

           
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<MedicineBatchResponseDto>(medBatch);
             



        }
    }
}
