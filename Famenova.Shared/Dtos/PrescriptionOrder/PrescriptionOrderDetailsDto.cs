using Famenova.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.PrescriptionOrder
{
    public class PrescriptionOrderDetailsDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = default!;
        public string PrescriptionImageUrl { get; set; } = default!;
        public AddressDto DeliveryAddress { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? RejectReason { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? OrderId { get; set; }
        public string? ClarificationRequest { get; set; }
        public string? ClarificationResponse { get; set; }
        public string? ClarificationImageUrl { get; set; }
        public DateTime? ClarificationRespondedAt { get; set; }
        public List<PrescriptionOrderItemDto> Items { get; set; } = new();
    }
}
