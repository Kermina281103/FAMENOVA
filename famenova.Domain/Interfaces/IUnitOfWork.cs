using famenova.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Interfaces
{
    public interface IUnitOfWork
    {
       IMedicineRepository MedicineRepository { get; }
        IPrescriptionOrderRepository PrescriptionOrderRepository { get; }
        IGenericRepository<TEntity> GetGeneric<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();

    }
}
