using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Responses
{
    public class ApiValidationResponse:ApiResponse
    {
       public List<string> Errors { get; set; }

        public static ApiValidationResponse ValidationFail(List<string> errors) =>
            new ApiValidationResponse
            {
                IsSuccess = false,
                StatusCode = 422,
                Message="One or More validation Error occur ",
                Errors = errors
            };
    }
}
