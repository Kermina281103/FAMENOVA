using Famenova.Application.Common.Models;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Shared.Dtos.Product;
using Microsoft.AspNetCore.Mvc;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController:ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("add-product")]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> CreateAsync(CreateProductDto dto)
        {
            var result =await _productService.CreateProductAsync(dto);
            return Ok(ApiResponse<ProductResponseDto>.Success(result, "Product Created Successfully"));
        }
        [HttpGet("get-all")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductResponseDto>>>> GetAllAsync(string?search , int? categoryId, string? sort , int pageIndex, int pageSize)
        {
            var result = await _productService.GetAllAsync(search, categoryId, sort, pageIndex, pageSize);
            return Ok(ApiResponse<PagedResult<ProductResponseDto>>.Success(result)); 
        }
        [HttpGet("get")]
        public async Task<ActionResult<ApiResponse>>GetByIdAsync(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(ApiResponse<ProductResponseDto>.Success(result));
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> UpdateAsync(int productId, UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync( productId, dto);
            return Ok(ApiResponse<ProductResponseDto>.Success(result));
        }
        [HttpDelete("delete")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(int id)
        {
            await  _productService.DeleteAsync(id);
            return Ok(ApiResponse<string>.Success("Deleted","ProductDeleted Successfully "));
        }
        [HttpPost("increase")]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> IncreaseStockAsync(int productId,int quantity)
        {
            var result = await _productService.IncreaseStockAsync(productId, quantity);
            return Ok(ApiResponse<ProductResponseDto>.Success(result, "stock increaced successfully"));
        }
        [HttpPost("adjust")]
        public async Task<ActionResult<ProductResponseDto>> AdjustStockAsync(int productId,int difference)
        {
            var result = await _productService.AdjustStockAsync(productId, difference);
            return Ok(ApiResponse<ProductResponseDto>.Success(result));
        }

    }
}
