using Famenova.Application.Common.Models;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Shared.Dtos.Medicine;
using Famenova.Shared.Dtos.MedicineBatch;
using Microsoft.AspNetCore.Mvc;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicineBatchController: ControllerBase
    {
        private readonly IMedicineBatchService _medicineBatchService;

        public MedicineBatchController(IMedicineBatchService medicineBatchService)
        {
            _medicineBatchService = medicineBatchService;
                
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<MedicineBatchResponseDto>>> CreateAsync(CreateMedicineBatchDto dto)
        {
            var result = await _medicineBatchService.CreateAsync(dto);
            return Ok(ApiResponse<MedicineBatchResponseDto>.Success(result, "Medicine Batch Added Successfully"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<MedicineBatchResponseDto>>> UpdateAsync(int id, UpdateMedicineBatchDto dto)
        {
            var result = await _medicineBatchService.UpdateAsync(id, dto);
            return Ok(ApiResponse<MedicineBatchResponseDto>.Success(result, "Medicine Batch Updated Successfully   "));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(int id)
        {
            await _medicineBatchService.DeleteAsync(id);
            return Ok(ApiResponse<string>.Success("Deleted", "Medicine Deleted Successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MedicineBatchResponseDto>>> GetByIdAsync(int id)
        {
            var result = await _medicineBatchService.GetByIdAsync(id);
            return Ok(ApiResponse<MedicineBatchResponseDto>.Success(result));
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<MedicineBatchResponseDto>>> GetAll(string? search, int? medicineId, string? sort, int pageIndex, int pageSize)
        {
            var result = await _medicineBatchService.GetAllAsync(search, medicineId,sort,pageIndex,pageSize);
            return Ok(ApiResponse<PagedResult<MedicineBatchResponseDto>>.Success(result, "Request is completed Successfully  "));
        }
    }
}
