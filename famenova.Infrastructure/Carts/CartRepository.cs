using famenova.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Carts
{
    class CartRepository : ICartRepository
    {
        private readonly IDatabase _database;
        public CartRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<Domain.Entities.Cart?> GetByIdAsync(string id)
        {
            var basket = await _database.StringGetAsync(id);

            return basket.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Domain.Entities.Cart>(basket!);
        }

        public async Task<Domain.Entities.Cart?> UpdateAsync(Domain.Entities.Cart cart,TimeSpan timeToLive)
        {
            var Jcart = JsonSerializer.Serialize(cart);
            var updated = await _database.StringSetAsync(cart.Id, Jcart, timeToLive);

            if (updated) return cart;
            return null;
        }
        public async Task<bool> DeleteAsync(string id)
        => await _database.KeyDeleteAsync(id);

       
    }
}
