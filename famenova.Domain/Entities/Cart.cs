using famenova.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class Cart
    {

        public string Id { get; set; } = default!;
        public IEnumerable<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
