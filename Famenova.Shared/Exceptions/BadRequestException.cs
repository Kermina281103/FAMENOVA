using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(message,400)
        {
        }
    }
}
