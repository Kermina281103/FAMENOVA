using Famenova.Application.Common.Models;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Application.Specifications;
using Famenova.Shared.Dtos.Order;
using Famenova.Shared.Dtos.PrescriptionOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionOrderController:ControllerBase
    {
        private readonly IPrescriptionOrderService _prescriptionOrderService;

        public PrescriptionOrderController(IPrescriptionOrderService prescriptionOrderService)
        {
            _prescriptionOrderService = prescriptionOrderService;   
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<PrescriptionOrderResponseDto>>> CreateAsyn(CreatePrescriptionOrderDto dto,int customerId)
        {
            var result = await _prescriptionOrderService.CreateAsync(dto, customerId);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result, "Prescription Order created successfully"));
        }
        [HttpGet("my")]
        public async Task<ActionResult<ApiResponse<PagedResult<PrescriptionOrderListItemDto>>>> GetMyAsync([FromQuery]PrescriptionOrderSpecParams dto , [FromQuery]int customerId)
        {
            var result = await _prescriptionOrderService.GetMyPrescriptionOrderAsync(dto, customerId);
            return Ok(ApiResponse<PagedResult<PrescriptionOrderListItemDto>>.Success(result, "Request is completed Succesfully"));
        }

        [HttpGet("myWithId")]
        public async Task<ActionResult<ApiResponse<PrescriptionOrderDetailsDto>>> GetMyWithIdAsync(int id,int customerId)
        {
            var result = await _prescriptionOrderService.GetMyPrescriptionOrderAsync(id, customerId);
            return Ok(ApiResponse<PrescriptionOrderDetailsDto>.Success(result, "Request Completed Succesffully"));
        }

        [HttpPost("approveCustomer")]
        public async Task<ActionResult<OrderResponseDto>> ApprovedByCustomerAsyn(int id , int customerId)
        {
            var result = await _prescriptionOrderService.ApproveByCustomerAsync(id, customerId);
            return Ok(ApiResponse<OrderResponseDto>.Success(result, "Request approved by customer"));
        }

        [HttpPost("rejectCustomer")]
        public async Task<ActionResult<PrescriptionOrderResponseDto>>RejectedByCustomerAsync(int id,int customerId)
        {
            var result = await _prescriptionOrderService.RejectByCustomerAsync(id, customerId);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result, "request rejected by customer"));
        }


        //Admind 

        [HttpGet("getAll")]
        public async Task<ActionResult<PagedResult<PrescriptionOrderListItemDto>>> GetAllAsync([FromQuery]PrescriptionOrderSpecParams specParams)
        {
            var result = await _prescriptionOrderService.GetAllAsync(specParams);
            return Ok(ApiResponse<PagedResult<PrescriptionOrderListItemDto>>.Success(result));
        }

        [HttpGet("getWithId")]
        public async Task<ActionResult<PrescriptionOrderDetailsDto>> GetByIdAsync(int id)
        {
            var result = await _prescriptionOrderService.GetByIdAsync(id);
            return Ok(ApiResponse<PrescriptionOrderDetailsDto>.Success(result));
        }
        [HttpPost("requestClarification")]
        public async Task<ActionResult<PrescriptionOrderResponseDto>> RequestClarification(int id, [FromBody] RequestClarificationDto dto)
        {
            var result = await _prescriptionOrderService.RequestClarificationAsync(id,dto.Message);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }
        [HttpPost("provideClarification")]
        public async Task<ActionResult<PrescriptionOrderResponseDto>> ProvideClarificationAsync(int id,int customerId, [FromBody] ProvideClarificationDto dto)
        {
            var result = await _prescriptionOrderService.ProvideClarificationAsync(id,customerId,dto);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));



        }

        [HttpPost("resume")]
        public async Task<ActionResult<PrescriptionOrderResponseDto>> ResumeReview(int id)
        {
            var result = await _prescriptionOrderService.ResumeReviewAsync(id);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }

        [HttpPost("rejectByAdmin")]
        public async Task<ActionResult<PrescriptionOrderResponseDto>> RejectByPharmacy(
            int id, RejectPrescriptionOrderDto rejectReason)
        {
            var result = await _prescriptionOrderService.RejectByPharmacyAsync(id,rejectReason);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }

        [HttpPost("addItem")]
        
        public async Task<ActionResult<PrescriptionOrderResponseDto>> AddItem(
            int id, [FromBody] CreatePrescriptionOrderItemDto dto)
        {
            var result = await _prescriptionOrderService.AddItemAsync(id,dto);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }

        [HttpDelete("removeItem")]
        
        public async Task<ActionResult<PrescriptionOrderResponseDto>> RemoveItem(int id, int itemId)
        {
            var result = await _prescriptionOrderService.RemoveItemAsync(id,itemId);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }
        [HttpPost("sendForApproval")]
       
        public async Task<ActionResult<PrescriptionOrderResponseDto>> SendForApproval(int id)
        {
            var result = await _prescriptionOrderService.SendForCustomerApprovaleAsync(id);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }

        // ================= Helpers =================

        private int GetCurrentUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

}

