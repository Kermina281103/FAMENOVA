using Famenova.Application.Common.Models;
using Famenova.Shared.Dtos.Medicine;
using Famenova.Shared.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
   public  interface IProductService
    {
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductResponseDto> UpdateAsync(int productId, UpdateProductDto dto);
        Task DeleteAsync(int id);
        Task<ProductResponseDto> GetByIdAsync(int id);
        Task<PagedResult<ProductResponseDto>> GetAllAsync(string? search, int? categoryId, string? sort, int pageIndex, int pageSize);
        Task<ProductResponseDto> IncreaseStockAsync(int productId, int quantity);
        Task<ProductResponseDto> AdjustStockAsync(int productId, int difference);

    }
}
