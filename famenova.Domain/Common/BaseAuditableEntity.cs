using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Common
{
    public abstract class BaseAuditableEntity<T>:BaseEntity<T>
    {
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string? LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; } = DateTime.UtcNow;
    }
}
