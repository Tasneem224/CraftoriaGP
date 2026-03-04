using DomainLayer.Contracts;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class AccountsController(IServiceManager _serviceManager):BaseApiController
    {
        [HttpGet("GetAccount")]
        public async Task<ActionResult<ReturnAccountDto>> GetAllAccounts(string userId)
        {
          return  SendSuccessResponse(await _serviceManager.accountService.GetAccount(userId),"Account is getted Successfully");
        }
        
        [HttpGet("GetReviewsOnUser")]
        public async Task<ActionResult<IEnumerable<ReviewsBelongToUSer>>> GetReviewsOnUser(string userId)
        {
           return SendSuccessResponse(await _serviceManager.accountService.GetReviewOfCustomer(userId),"Account is getted Successfully");
        }
        
    }
}
