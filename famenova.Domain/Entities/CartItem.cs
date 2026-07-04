using famenova.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class CartItem:BaseAuditableEntity<int>
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public Cart Cart { get; set; } = default!;
        public int CartId { get; set; }
        public int NumberOfItem { get; set; }
    }
}
