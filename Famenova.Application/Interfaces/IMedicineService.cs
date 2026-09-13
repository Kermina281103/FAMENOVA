using Famenova.Application.Common.Models;
using Famenova.Shared.Dtos.Medicine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface  IMedicineService
    {
        Task<MedicineResponseDto> CreateAsync( CreateMedicineDto dto);
        Task<MedicineResponseDto> UpdateAsync(int medicineId, UpdateMedicineDto dto);
        Task DeleteAsync(int id);
        Task<MedicineResponseDto> GetByIdAsync(int id);
        Task<PagedResult<MedicineResponseDto>> GetAllAsync(string? search,int? categoryId,bool? requiresPrescription, string? sort, int pageIndex, int pageSize);

    }
}
