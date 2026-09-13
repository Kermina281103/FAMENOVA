using Famenova.Application.Common.Models;
using Famenova.Application.Dtos.Category;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Shared.Dtos.Medicine;
using Microsoft.AspNetCore.Mvc;

namespace Famenova.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class MedicineController:ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicineController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<MedicineResponseDto>>> CreateAsync(CreateMedicineDto dto)
        {
            var result = await _medicineService.CreateAsync(dto);
            return Ok(ApiResponse<MedicineResponseDto>.Success(result, "Medicine Added Successfully"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<MedicineResponseDto>>> UpdateAsync(int id, UpdateMedicineDto dto)
        {
            var result = await _medicineService.UpdateAsync(id, dto);
            return Ok(ApiResponse<MedicineResponseDto>.Success(result, "Medicine Updated Successfully   "));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(int id)
        {
            await _medicineService.DeleteAsync(id);
            return Ok(ApiResponse<string>.Success("Deleted", "Medicine Deleted Successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MedicineResponseDto>>> GetByIdAsync(int id)
        {
            var result = await _medicineService.GetByIdAsync(id);
            return Ok(ApiResponse<MedicineResponseDto>.Success(result));
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<MedicineResponseDto>>> GetAll(string? search, int? categoryId, bool? requiresPrescription, string? sort, int pageIndex, int pageSize)
        {
            var result = await _medicineService.GetAllAsync(search,categoryId,requiresPrescription,sort,pageIndex,pageSize);
            return Ok(ApiResponse<PagedResult<MedicineResponseDto>>.Success(result, "Request is completed Successfully  "));
        }
    }
}
