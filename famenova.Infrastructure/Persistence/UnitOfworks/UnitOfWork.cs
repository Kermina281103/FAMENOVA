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
        private readonly Dictionary<Type, object> _repositories = [];
        public IGenericRepository<TEntity> GetGeneric<TEntity>() where TEntity : class
        {
            var TypeOfName = typeof(TEntity);
            if (_repositories.ContainsKey(TypeOfName))
            {
                return (IGenericRepository<TEntity>)_repositories[TypeOfName];
            }
            else
            {
                var NewObjetct = new GenericRepository<TEntity>(_context);
                _repositories[TypeOfName] = NewObjetct;
                return NewObjetct;
            }
        }

        public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();
    }
}
