using famenova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Specifications.param
{
    public class OrderSpecParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int pageIndex { get; set; }
        public int  pageSize { get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public OrderStatus? orderStatus { get; set; }
        public PaymentStatus? paymentStatus { get; set; }
        public PaymentMethod? paymentMethod { get; set; }
        public OrderSource? source { get; set; }

        public int? customerId { get; set; }
        public string? search { get; set; }
        public string? sort { get; set; }
    }
}
