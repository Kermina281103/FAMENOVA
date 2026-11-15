using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message="Access Denied") : 
            base(message, 403)
        {
        }
    }
}
