using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message="You are UnAuthorized") : base(message, 401)
        {
        }
    }
}
