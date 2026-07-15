using famenova.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Interfaces
{
    public interface IPrescriptionOrderRepository:IGenericRepository<PrescriptionOrder>
    {
        Task<PrescriptionOrder?> GetPrescriptionOrderWithDetailsAsync(int id);
    }
}
