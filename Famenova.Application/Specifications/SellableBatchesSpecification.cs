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
    public class SellableBatchesSpecification : BaseSpecification<MedicineBatch>
    {
        public SellableBatchesSpecification(IEnumerable<int> medicineIds)
        {
            var ids = medicineIds.ToList();
            var today = DateTime.UtcNow.Date;
            Criteria = b => ids.Contains(b.MedicineId) && b.Quantity > 0 && b.ExpiryDate > today;
            AddOrderBy(b => b.ExpiryDate);
        }
    }

  
}
