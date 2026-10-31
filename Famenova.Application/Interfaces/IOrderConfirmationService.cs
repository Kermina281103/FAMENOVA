using famenova.Domain.Entities;
using famenova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface IOrderConfirmationService
    {
        Task ConfirmAsync(Order order, PaymentMethod method);
        Task ReleaseStockAsync(Order order);
        Task EnsureStockAvailableAsync(Order order);
    }
}
