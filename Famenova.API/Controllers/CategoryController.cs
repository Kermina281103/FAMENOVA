using Famenova.Application.Common.Models;
using Famenova.Application.Dtos.Category;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

  //  [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles = "Admin")]
    public class CategoryController:ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>>  CreateAsync([FromForm] CreateCategoryDto dto)
        {
           
            var result = await _categoryService.CreateAsync(dto);
            return Ok(ApiResponse<CategoryResponseDto>.Success(result, "Category Added Successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> UpdateAsync(int id ,[FromForm]UpdateCategoryDto dto)
        {
            
            var result = await _categoryService.UpdateAsync(id,dto);
            return Ok(ApiResponse<CategoryResponseDto>.Success(result, "Category Updated Successfully   "));
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(int id)
        {
           await _categoryService.DeleteAsync(id);
            return Ok(ApiResponse<string>.Success("Deleted", "Category Deleted Successfully"));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> GetByIdAsync(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return Ok(ApiResponse<CategoryResponseDto>.Success(result));
        }
        [HttpGet]
        [AllowAnonymous]
       public async Task<ActionResult<ApiResponse<PagedResult<CategoryResponseDto>>>> GetAll(string?search , string?sort,int pageIndex=1,int pageSize=10)
        {
            var result =await  _categoryService.GetAllAsync(search, sort, pageIndex, pageSize);
            return Ok(ApiResponse<PagedResult<CategoryResponseDto>>.Success(result, "Request is completed Successfully  "));
        }
    }
}
