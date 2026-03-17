using DomainLayer.Models.Items;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using Service;
using ServiceAbstraction;
using Shared.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class CartsController:BaseApiController
    {
        private readonly IServiceManager _serviceManager;
        public CartsController (IServiceManager serviceManager)
        {
            _serviceManager=serviceManager;
        }

        [HttpGet("GetCartUser")]
        public async Task<ActionResult<CartDto>> GetCartAsync(string id)=>
             SendSuccessResponse(await _serviceManager.CartService.GetCartAsync(id), "Car was gotten successfully");
        [HttpPost("AddItem")]
        public async Task<ActionResult<CartDto>> AddItem(string cartId, int  itemId)  => 
            SendSuccessResponse(await _serviceManager.CartService.AddItemToCartAsync(cartId, itemId));
        [HttpPatch("Quantity")]
        public async Task<ActionResult<CartDto>> UpdateQuantity(string cartId, int productId, bool isIncrement) =>
            SendSuccessResponse(await _serviceManager.CartService.UpdateQuantityAsync(cartId, productId, isIncrement));
       
        [HttpDelete("DeleteItem")]
        public async Task<ActionResult> DeleteAsync(string cartId,int ItemId) =>
              SendSuccessResponse(await _serviceManager.CartService.RemoveItemFromCartAsync(cartId, ItemId),"Cart was Deleted successfully");

        [HttpGet("GetCountOfCart")]
        public async Task<ActionResult> Count(string id) => SendSuccessResponse(await _serviceManager.CartService.GetCountOfCart(id));     
    }
}
 