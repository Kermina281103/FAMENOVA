using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using famenova.Infrastructure.Data.Context;
using famenova.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Persistence.UnitOfworks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IMedicineRepository MedicineRepository { get; private set; }

        public IPrescriptionOrderRepository PrescriptionOrderRepository { get; private set; }

        public UnitOfWork(AppDbContext context,IMedicineRepository medicineRepository,IPrescriptionOrderRepository prescriptionOrderRepository )
        {
            _context = context;
            MedicineRepository = medicineRepository;
            PrescriptionOrderRepository = prescriptionOrderRepository;
        }
        private readonly Dictionary<string, object> Map = [];
        public IGenericRepository<TEntity> GetGeneric<TEntity>() where TEntity : class
        {
            var TypeOfName = typeof(TEntity).Name;
            if (Map.ContainsKey(TypeOfName))
            {
                return (IGenericRepository<TEntity>)Map[TypeOfName];
            }
            else
            {
                var NewObjetct = new GenericRepository<TEntity>(_context);
                Map[TypeOfName] = NewObjetct;
                return NewObjetct;
            }
        }

        public async Task<int> SaveChanges()
        => await _context.SaveChangesAsync();
    }
}
