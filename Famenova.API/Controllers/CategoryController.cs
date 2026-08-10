using Famenova.Application.Common.Models;
using Famenova.Application.Dtos.Category;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController:ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>>  CreateAsync(CreateCategoryDto dto)
        {
            var result = await _categoryService.CreateAsync(dto);
            return Ok(ApiResponse<CategoryResponseDto>.Success(result, "Category Added Successfully"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> UpdateAsync(int id ,UpdateCategoryDto dto)
        {
            var result = await _categoryService.UpdateAsync(id,dto);
            return Ok(ApiResponse<CategoryResponseDto>.Success(result, "Category Updated Successfully   "));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(int id)
        {
           await _categoryService.DeleteAsync(id);
            return Ok(ApiResponse<string>.Success("Deleted", "Category Deleted Successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> GetByIdAsync(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return Ok(ApiResponse<CategoryResponseDto>.Success(result));
        }
        [HttpGet]
       public async Task<ActionResult<PagedResult<CategoryResponseDto>>> GetAll(string?search , string?sort,int pageIndex=1,int pageSize=10)
        {
            var result =await  _categoryService.GetAllAsync(search, sort, pageIndex, pageSize);
            return Ok(ApiResponse<PagedResult<CategoryResponseDto>>.Success(result, "Request is completed Successfully  "));
        }
    }
}
