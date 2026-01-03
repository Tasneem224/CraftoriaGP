using DomainLayer.Exceptions;
using DomainLayer.Exceptions.DomainLayer.Exceptions;
using Shared;
using Shared.ErrorModels;
using System.Text.Json;

namespace CraftoriaApp.CustomeMiddleWares
{
    public class CustomeExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomeExceptionHandlerMiddleWare> _logger;

        public CustomeExceptionHandlerMiddleWare(RequestDelegate next,ILogger<CustomeExceptionHandlerMiddleWare> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                //Request
                await _next.Invoke(httpContext);
                //Response

            }
            
                catch (Exception ex)
            {
                int statusCode = ex switch
                {
                    BadRequestException=> StatusCodes.Status400BadRequest,
                    UserAlreadyExistsException => StatusCodes.Status400BadRequest,
                    NotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedAException => StatusCodes.Status401Unauthorized,
                    InvalidException=>StatusCodes.Status422UnprocessableEntity,
                    _ => StatusCodes.Status400BadRequest
                };

                _logger.LogError(ex, "Something went wrong while processing.");

                var errorModel = new ErrorToReturn
                {
                    StatusCode = statusCode,
                    ErrorMessage = ex.Message
                };

                var response = ApiResponse<string>.FailResponse(
                    ex is UserAlreadyExistsException ? ex.Message : "Internal Server Error",
                    errorModel,
                    statusCode.ToString()
                );

              
            await httpContext.Response.WriteAsJsonAsync(response);//convert content type and Serialize response and writeasync
            }


            
        }
    }
}
