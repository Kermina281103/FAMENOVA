using famenova.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class Category:BaseAuditableEntity<int>
    {
        public string Name { get; set; } = default!;
        public string? ImageUrl { get; set; } 
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }

        public ICollection<Product> products { get; set; } = new HashSet<Product>();
    }
}
