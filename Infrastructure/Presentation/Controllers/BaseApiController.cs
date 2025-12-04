using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected ActionResult SendSuccessResponse<T>(T data, string message = "Do Successfully")
        {
            return Ok(ApiResponse<T>.SuccessResponse(data, message));
        }


        protected IActionResult SendErrorResponse(string message, object errors = null, int statusCode = 400)
        {
            var response = ApiResponse<object>.FailResponse(message, errors);

            return new ObjectResult(response)
            {
                StatusCode = statusCode
            };
        }

    }
}
