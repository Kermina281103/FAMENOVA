using Famenova.Application.Common.Models;
using Famenova.Application.Specifications.param;
using Famenova.Shared.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDetailsDto> CreateOrderFromCartAsync(int userId, CreateOrderFromCartDto dto);
        Task<OrderDetailsDto> CreateOrderFromPrescriptionAsync(int userId, int prescriptionId);
        Task<PagedResult<OrderListItemDto>> GetMyOrdersAsync(OrderSpecParams spec,int userId);
        Task<OrderDetailsDto> GetMyOrderWithIdAsync(int id, int userId);
        Task<PagedResult<OrderListItemDto>> GetAllAsync(OrderSpecParams spec);
        Task<OrderDetailsDto> GetByIdAsync(int id);
        Task<OrderDetailsDto> CancelMyOrderAsync(int id, int userId);
    }
}
