using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Famenova.Application.Responses
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = default!;
        public List<string> Errors { get; set; } = [];

        public static ApiResponse Failure(string error, int statusCode=400) => new()
        {
            IsSuccess=false,
            StatusCode=statusCode,
            Message=error,
            Errors = {error}

        };

        public override string ToString()
       => JsonSerializer.Serialize(this);
    }

public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T? data, string message="Request is completed Successfully") => new()
        {
            IsSuccess=true,
            StatusCode=200,
            Message=message,
            Data=data

        };
    }
}
