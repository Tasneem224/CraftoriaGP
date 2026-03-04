using Shared.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IAccountService
    {
        Task<ReturnAccountDto> GetAccount(string UserId);
        Task<IEnumerable<ReviewsBelongToUSer>>GetReviewOfCustomer(string CustomerId);
    }
}
