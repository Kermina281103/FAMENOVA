using famenova.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Interfaces
{
    public interface IMedicineRepository:IGenericRepository<Medicine>
    {
        Task<Medicine?> GetMedicineWithBatchAsync(int id );
    }
}
