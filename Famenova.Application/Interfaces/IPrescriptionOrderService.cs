using famenova.Domain.Enums;
using Famenova.Application.Common.Models;
using Famenova.Application.Specifications;
using Famenova.Shared.Dtos.Order;
using Famenova.Shared.Dtos.PrescriptionOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface IPrescriptionOrderService
    {
        //Customer
        Task<PrescriptionOrderResponseDto> CreateAsync(CreatePrescriptionOrderDto dto, int  customerId);

        Task<PrescriptionOrderResponseDto> RejectByCustomerAsync(int id, int customerId);
        Task<OrderDetailsDto> ApproveByCustomerAsync(int id, int customerId);
        Task<PagedResult<PrescriptionOrderListItemDto>> GetMyPrescriptionOrderAsync(PrescriptionOrderSpecParams dto, int customerId);
        Task<PrescriptionOrderDetailsDto> GetMyPrescriptionOrderAsync(int id, int customerId);

        //Admin
        Task<PrescriptionOrderResponseDto> RequestClarificationAsync(int id, string message, int? pharmacistId = null);
        Task<PrescriptionOrderResponseDto> ProvideClarificationAsync(int id, int customerId, ProvideClarificationDto dto);
        Task<PrescriptionOrderResponseDto> ResumeReviewAsync(int id);
        Task<PrescriptionOrderResponseDto> RejectByPharmacyAsync(int id, RejectPrescriptionOrderDto rejectReason);
        Task<PrescriptionOrderResponseDto> AddItemAsync(int id, CreatePrescriptionOrderItemDto dto);
        Task<PrescriptionOrderResponseDto> RemoveItemAsync(int id, int itemId);
        Task<PrescriptionOrderResponseDto> SendForCustomerApprovaleAsync(int Id);
        Task<PagedResult<PrescriptionOrderListItemDto>> GetAllAsync(PrescriptionOrderSpecParams specParams);
        Task<PrescriptionOrderDetailsDto> GetByIdAsync(int id);

        // Conversation (both sides). asPharmacist = the caller is staff (Admin role).
        Task<IReadOnlyList<PrescriptionMessageDto>> GetMessagesAsync(int id, int userId, bool asPharmacist);
        Task<PrescriptionMessageDto> SendMessageAsync(int id, int userId, bool asPharmacist, SendPrescriptionMessageDto dto);
        Task<int> MarkMessagesReadAsync(int id, int userId, bool asPharmacist);
        Task<UnreadMessagesSummaryDto> GetUnreadAsync(int userId, bool asPharmacist);
    }
}
