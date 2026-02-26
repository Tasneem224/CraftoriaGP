using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Attributes
{
     //Attribute, IAsyncActionFilter
    internal class RedisCacheAttribute :ActionFilterAttribute
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
                    //StatusCode = Status,
                };
            
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
