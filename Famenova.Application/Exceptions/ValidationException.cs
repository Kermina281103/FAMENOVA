using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Exceptions
{
    public class ValidationException:BaseException
    {
        public List<string> Errors { get; }

        public ValidationException(List<string> errors):base("One or more validation error occur ",422)
        {
            Errors = errors;
        }
    }
}
