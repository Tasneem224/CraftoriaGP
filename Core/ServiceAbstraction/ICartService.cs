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
        Task<CartDto> AddItemToCartAsync(string cartId, int itemId);
        Task<CartDto> UpdateQuantityAsync(string cartId, int productId, bool isIncrement);
        Task<CartDto> RemoveItemFromCartAsync(string cartId, int productId);
        Task<CartDto> GetCartAsync(string id);
        Task<int> GetCountOfCart(string id);
       
    }
}
