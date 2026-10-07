using famenova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Specifications
{
    public class PrescriptionOrderSpecParams
    {
        public string? search { get; set; }
        public int? customerId { get; set; }
        public PrescriptionStatus? status { get; set; }
        public string? sort { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public  bool? HasClarificationResponse { get; set; }
    }
}
