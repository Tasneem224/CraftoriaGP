using Microsoft.OpenApi.Models;
using Shared;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CraftoriaApp.Helper
{
    public class DefaultResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // تعريف شكل الـ Error Response الموحد بناءً على الـ Middleware الخاص بكِ
            var errorSchema = context.SchemaGenerator.GenerateSchema(typeof(ApiResponse<string>), context.SchemaRepository);

            // إضافة 400 BadRequest (تغطي BadRequest, UserAlreadyExists, وغيرها في الميدل وير)
            if (!operation.Responses.ContainsKey("400"))
                operation.Responses.Add("400", new OpenApiResponse
                {
                    Description = "Bad Request - Validation or Logic Error",
                    Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType { Schema = errorSchema } }
                });

            // إضافة 404 NotFound
            if (!operation.Responses.ContainsKey("404"))
                operation.Responses.Add("404", new OpenApiResponse { Description = "Resource Not Found" });

            // إضافة 401 Unauthorized
            if (!operation.Responses.ContainsKey("401"))
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized - Invalid Token" });

            // إضافة 422 UnprocessableEntity (للـ InvalidException عندك)
            if (!operation.Responses.ContainsKey("422"))
                operation.Responses.Add("422", new OpenApiResponse { Description = "Validation Error - Unprocessable Entity" });
        }
    }
}

