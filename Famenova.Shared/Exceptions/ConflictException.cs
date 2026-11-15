using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared
{
    public class ConflictException : BaseException
    {
        public ConflictException(string message) : base(message, 409)
        {
        }
    }
}
