using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using famenova.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Persistence
{
    public class MedicineRepository(AppDbContext _context) : GenericRepository<Medicine>(_context), IMedicineRepository
    {
        public async Task<Medicine?> GetMedicineWithBatchAsync(int id)
        {
            return await _context.Set<Medicine>().Include(x => x.Batches).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
