using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Common
{
    public enum PrescriptionStatusDto
    {
        PendingReview,
        NeedsClarification,
        RejectedByPharmacy,
        AwaitingCustomerApproval,
        RejectedByCustomer,
        Confirmed,
        Preparing,
        OutForDelivery,
        Delivered,
        Cancelled
    }
}
