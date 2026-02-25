using Shared.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public  interface ICartService
    {
        Task<CartDto> GetCartAsync(string id);
        Task<bool> DeleteCartAsync(string id);
        Task<CartDto> CreateOrUpdateCartAsync(CartDto cartDto);
    }
}
