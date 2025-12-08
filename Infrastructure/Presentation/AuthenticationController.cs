using FluentValidation;
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
            
                var user = await _serviceManager.AuthenticationService.RegisterAsync(_customerRegisterDto);
                return SendSuccessResponse(user, "Registration successful");
        }
    }
    }
