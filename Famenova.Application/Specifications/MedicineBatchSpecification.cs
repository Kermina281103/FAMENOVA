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
    public class MedicineBatchSpecification:BaseSpecification<MedicineBatch>
    {
        public MedicineBatchSpecification(string? search , int? medicineId,string? sort , int pageIndex, int pageSize)
        {
            Criteria = c => (string.IsNullOrEmpty(search) || c.BatchNumber.Contains(search))
            && (!medicineId.HasValue || c.MedicineId == medicineId.Value);

            AddInclude(query => query.Include(x => x.Medicine));

            if (String.IsNullOrEmpty(sort))
            {
                AddOrderBy(c => c.BatchNumber);
            }
            else
            {
                switch (sort.ToLower())
                {
                    case "medicineid":
                        AddOrderBy(c => c.MedicineId);
                          break;
                    case "medicineiddesc":
                        AddOrderByDescending(c => c.MedicineId);
                        break;
                    case "expirydate":
                        AddOrderBy(c => c.ExpiryDate);
                        break;
                    case "expirydatedesc":
                        AddOrderByDescending(c => c.ExpiryDate);
                        break;
                    case "batchnumberdesc":
                        AddOrderByDescending(c => c.BatchNumber);
                        break;
                    default:
                        AddOrderBy(c => c.BatchNumber);
                        break;
                }
            }
            ApplyPaging(pageIndex, pageSize);
        }

        public MedicineBatchSpecification(int id)
        {
            Criteria = c => c.Id == id;
            AddInclude(query => query.Include(x => x.Medicine));
        }
    }
}
