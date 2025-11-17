using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meli.Products.Application.Exceptions
{
    public class ApiException: Exception
    {
        public int StatusCode { get; }
        public string? Details { get; }

        public ApiException(string message, int statusCode = 500, string? details = null)
       : base(message)
        {
            StatusCode = statusCode;
            Details = details;
        }

        // Métodos helper para crear excepciones comunes
        public static ApiException NotFound(string message)
            => new ApiException(message, 404);

        public static ApiException BadRequest(string message)
            => new ApiException(message, 400);

        public static ApiException ValidationError(string message, string details)
            => new ApiException(message, 400, details);
    }
}
