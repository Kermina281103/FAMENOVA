using famenova.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByIdAsync(string id);

        Task<Cart?> UpdateAsync(Cart cart,TimeSpan timeToLive);
        Task<bool> DeleteAsync(string id);
    }
}
