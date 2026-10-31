using famenova.Domain.Entities;
using Famenova.Shared.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Specifications
{
    public class PaymentByMerchantRefSpecification:BaseSpecification<Payment>
    {
        public PaymentByMerchantRefSpecification(string merchantRefNumber)
        {
            Criteria = p => p.MerchantRefNumber == merchantRefNumber;
            AddInclude(q => q.Include(p => p.Order).ThenInclude(o => o.OrderItems));
        }
    }
}
