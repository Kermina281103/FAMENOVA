using famenova.Domain.Constants;
using Famenova.Application.Common.Models;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Application.Specifications;
using Famenova.Shared.Dtos.Order;
using Famenova.Shared.Dtos.PrescriptionOrder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        [Consumes("multipart/form-data")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles =AppRoles.Customer)]
        public async Task<ActionResult<ApiResponse<PrescriptionOrderResponseDto>>> CreateAsyn([FromForm]CreatePrescriptionOrderDto dto)
        {
            var uid = User.GetUserId();

            var result = await _prescriptionOrderService.CreateAsync(dto, uid);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result, "Prescription Order created successfully"));
        }
        [HttpGet("my")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Customer)]

        public async Task<ActionResult<ApiResponse<PagedResult<PrescriptionOrderListItemDto>>>> GetMyAsync([FromQuery]PrescriptionOrderSpecParams dto , [FromQuery]int customerId)

        {
            var uid = User.GetUserId();
            var result = await _prescriptionOrderService.GetMyPrescriptionOrderAsync(dto, uid);
            return Ok(ApiResponse<PagedResult<PrescriptionOrderListItemDto>>.Success(result, "Request is completed Succesfully"));
        }

        [HttpGet("myWithId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Customer)]
        public async Task<ActionResult<ApiResponse<PrescriptionOrderDetailsDto>>> GetMyWithIdAsync(int id)
        {
            var result = await _prescriptionOrderService.GetMyPrescriptionOrderAsync(id, User.GetUserId());
            return Ok(ApiResponse<PrescriptionOrderDetailsDto>.Success(result));
        }

        [HttpPost("approveCustomer")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Customer)]
        public async Task<ActionResult<OrderDetailsDto>> ApprovedByCustomerAsyn(int id , int customerId)
        {
            var uid = User.GetUserId();

            var result = await _prescriptionOrderService.ApproveByCustomerAsync(id, uid);
            return Ok(ApiResponse<OrderDetailsDto>.Success(result, "Request approved by customer"));
        }

        [HttpPost("rejectCustomer")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Customer)]

        public async Task<ActionResult<PrescriptionOrderResponseDto>>RejectedByCustomerAsync(int id)
        {
            var uid = User.GetUserId();
            var result = await _prescriptionOrderService.RejectByCustomerAsync(id, uid);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result, "request rejected by customer"));
        }


        //Admind 

        [HttpGet("getAll")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Admin)]

        public async Task<ActionResult<PagedResult<PrescriptionOrderListItemDto>>> GetAllAsync([FromQuery]PrescriptionOrderSpecParams specParams)
        {
            var result = await _prescriptionOrderService.GetAllAsync(specParams);
            return Ok(ApiResponse<PagedResult<PrescriptionOrderListItemDto>>.Success(result));
        }

        [HttpGet("getWithId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Admin)]

        public async Task<ActionResult<PrescriptionOrderDetailsDto>> GetByIdAsync(int id)
        {
            var result = await _prescriptionOrderService.GetByIdAsync(id);
            return Ok(ApiResponse<PrescriptionOrderDetailsDto>.Success(result));
        }
        [HttpPost("requestClarification")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Admin)]

        public async Task<ActionResult<PrescriptionOrderResponseDto>> RequestClarification(int id, [FromBody] RequestClarificationDto dto)
        {
            var result = await _prescriptionOrderService.RequestClarificationAsync(id,dto.Message);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }
        [HttpPost("provideClarification")]
        [Consumes("multipart/form-data")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Customer)]
        public async Task<ActionResult<ApiResponse<PrescriptionOrderResponseDto>>> ProvideClarificationAsync(
       int id, [FromForm] ProvideClarificationDto dto)
        {
            var result = await _prescriptionOrderService.ProvideClarificationAsync(id, User.GetUserId(), dto);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result, "Clarification sent"));
        }

        [HttpPost("resume")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Admin)]

        public async Task<ActionResult<PrescriptionOrderResponseDto>> ResumeReview(int id)
        {
            var result = await _prescriptionOrderService.ResumeReviewAsync(id);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }

        [HttpPost("rejectByAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Admin)]

        public async Task<ActionResult<PrescriptionOrderResponseDto>> RejectByPharmacy(
            int id, RejectPrescriptionOrderDto rejectReason)
        {
            var result = await _prescriptionOrderService.RejectByPharmacyAsync(id,rejectReason);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }

        [HttpPost("addItem")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Admin)]

        public async Task<ActionResult<PrescriptionOrderResponseDto>> AddItem(
            int id, [FromBody] CreatePrescriptionOrderItemDto dto)
        {
            var result = await _prescriptionOrderService.AddItemAsync(id,dto);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }

        [HttpDelete("removeItem")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Admin)]

        public async Task<ActionResult<PrescriptionOrderResponseDto>> RemoveItem(int id, int itemId)
        {
            var result = await _prescriptionOrderService.RemoveItemAsync(id,itemId);
            return Ok(ApiResponse<PrescriptionOrderResponseDto>.Success(result));
        }
        [HttpPost("sendForApproval")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppRoles.Admin)]
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

