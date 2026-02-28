using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Attributes
{
     //Attribute, IAsyncActionFilter
    internal class RedisCacheAttribute(int _durarionInSeconds=200) :ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService=context.HttpContext.RequestServices.GetRequiredService<IServiceManager>().cacheService;

            string key = GenerateKey(context.HttpContext.Request);
            var cachedData =await cacheService.GetCacheValueAsync<string>(key);
            if (cachedData != null) {
                context.Result = new ContentResult
                {
                    Content=cachedData,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK,
                };
                return;
            
            }
            var resultContext=await next.Invoke();
            if (resultContext.Result is OkObjectResult okObjectResult) {
                var serializedData = JsonSerializer.Serialize(okObjectResult.Value);

                await cacheService.SetCacheValueAsync(key, serializedData, TimeSpan.FromSeconds(_durarionInSeconds));
            
            }
        }

        private string GenerateKey(HttpRequest request)
        {
            StringBuilder key = new StringBuilder();

            key.Append(request.Path);
            foreach(var item in request.Query.OrderBy(q => q.Key))
            {
                key.Append($"{item.Key}-{item.Value}");
            }
            return key.ToString();


        }
    }
}
