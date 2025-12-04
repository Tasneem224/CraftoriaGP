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
                _logger.LogError(ex,"Something went wrong while processing.");
                //set status code for response
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                ///httpContext.Response.ContentType = "application/json";
                ///var jsonResponse = JsonSerializer.Serialize(response);
                ///await httpContext.Response.WriteAsync(jsonResponse);
                var errorModel = new ErrorToReturn
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = ex.Message
                };

                var response = ApiResponse<string>.FailResponse(
                    "Internal Server Error",
                    errorModel,
                    StatusCodes.Status500InternalServerError.ToString()
                );


                await httpContext.Response.WriteAsJsonAsync(response);//convert content type and Serialize response and writeasync
            }
        }
    }
}
