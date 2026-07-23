using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message="Access Denied") : 
            base(message, 403)
        {
        }
    }
}
