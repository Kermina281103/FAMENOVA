using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using famenova.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Persistence.Repositories
{
    public class PrescriptionOrderRepository(AppDbContext _context) : GenericRepository<PrescriptionOrder>(_context), IPrescriptionOrderRepository
    {
        public async Task<PrescriptionOrder?> GetPrescriptionOrderWithDetailsAsync(int id)
        {
            return await _context.Set<PrescriptionOrder>().Include(d => d.Customer)
                .Include(d=>d.Items).FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
