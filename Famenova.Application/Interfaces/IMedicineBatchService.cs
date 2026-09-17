using Famenova.Application.Common.Models;
using Famenova.Shared.Dtos.MedicineBatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface IMedicineBatchService
    {
        Task<MedicineBatchResponseDto> CreateAsync(CreateMedicineBatchDto dto);
        Task<MedicineBatchResponseDto> UpdateAsync(int batchId,UpdateMedicineBatchDto dto);
        Task<PagedResult<MedicineBatchResponseDto>> GetAllAsync(string? search, int? medicineId,string? sort, int pageIndex, int pageSize);
        Task<MedicineBatchResponseDto> GetByIdAsync(int id);

        Task DeleteAsync(int id);
    }
}
