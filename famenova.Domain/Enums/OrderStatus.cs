using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Enums
{
    public enum OrderStatus
    {
        Pending=0,
        Processing=1,
        Shipped=2 ,//ف الطريق 
        Delivered=3,
        Cancelled=4,
        Returned=5
       
    }
    public enum OrderType
    {
        Prescription = 1,
        Cart = 2
    }

}
