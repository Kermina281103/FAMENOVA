using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Enums
{
    public enum PaymentAttemptStatus
    {
        Pending=0,
        Succeeded=1,
        Failed=2,
        Expired=3,
        Refunded=4
    }
}
