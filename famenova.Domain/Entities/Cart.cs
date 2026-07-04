using famenova.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class Cart:BaseAuditableEntity<int>
    {
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
    }
}
