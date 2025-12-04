using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.IdentityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class AuthenticationController(IServiceManager _serviceManager) : BaseApiController
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Registeration(RegisterDto _customerRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                return SendErrorResponse("Validation failed", ModelState, 422);
            }
            try
            {
                var user = await _serviceManager.AuthenticationService.RegisterAsync(_customerRegisterDto);

                return SendSuccessResponse(user, "Registration successful");
            }
            catch (Exception ex)
            {
                return SendErrorResponse(ex.Message, null, 400);
            }
        }
    }
    }
