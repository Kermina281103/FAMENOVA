using famenova.Domain.Common;
using famenova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    //الروشته 
    public class PrescriptionOrder:BaseAuditableEntity<int>
    {
        public string PrescriptionImageUrl { get; set; } = default!;
        public Address DeliveryAddress { get; set; }=default!;
        public string PhoneNumber { get; set; } = default!;

        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Pending;

        public string? RejectReason { get; set; }
        public decimal? TotalPrice { get; set; }

        public int CustomerId { get; set; }
        public ApplicationUser Customer { get; set; } = default!;

        public ICollection<PrescriptionOrderItem> Items { get; set; } = new HashSet<PrescriptionOrderItem>();



    }
}
