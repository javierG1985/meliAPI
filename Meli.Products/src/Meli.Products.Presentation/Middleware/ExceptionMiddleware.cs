using Meli.Products.Application.Exceptions;

namespace Meli.Products.Presentation.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Error no controlado: {Message}", exception.Message);

            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = "Ha ocurrido un error",
                error = new
                {
                    message = exception.Message,
                    details = _env.IsDevelopment() ? exception.StackTrace : null
                }
            };

            // Manejar ApiException personalizada
            if (exception is ApiException apiException)
            {
                context.Response.StatusCode = apiException.StatusCode;
                response = new
                {
                    success = false,
                    message = apiException.Message,
                    error = new
                    {
                        message = apiException.Message,
                        details = apiException.Details
                    }
                };
            }
            else
            {
                context.Response.StatusCode = 500;
            }

            await context.Response.WriteAsJsonAsync(response);
        }

    }
}
