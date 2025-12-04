using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }

        public T Data { get; set; }

        // "object" هنا هو الحل السحري
        public object Errors { get; set; }
        public static ApiResponse<T> SuccessResponse(T data, string message = "Successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null,

            };
        }
        public static ApiResponse<T> FailResponse(string message, object errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors,
            };
        }
    }
}
