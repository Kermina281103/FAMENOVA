using Famenova.Application.Common.Models;
using Famenova.Application.Dtos.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);

        Task<CategoryResponseDto> UpdateAsync(int categoryId, UpdateCategoryDto dto);

        Task DeleteAsync(int id);

        Task<CategoryResponseDto> GetByIdAsync(int categoryId);
        Task<PagedResult<CategoryResponseDto>> GetAllAsync(string? search, string? sort, int pageIndex, int pageSize);
    }
}
