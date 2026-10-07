using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Enums
{
    public enum PrescriptionStatus
    {
        PendingReview = 0,
        NeedsClarification = 1,
        RejectedByPharmacy = 2,
        AwaitingCustomerApproval = 3,
        RejectedByCustomer = 4,
        ApprovedByCustomer = 5

    }
}
