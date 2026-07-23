using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Exceptions
{
    public class NotFoundException:BaseException
    {
        public NotFoundException(string message):base($"Not found",404)
        {
            
        }
    }
}
