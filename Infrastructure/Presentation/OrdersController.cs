using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IServiceManager _orderService;
        public OrdersController(IServiceManager serviceManager) =>
            _orderService = serviceManager;
        
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(int deliveryMethodId, string basketId, AddressBookDto shippingAddress)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return SendSuccessResponse(await _orderService.orderService.CreateOrderAsync(email, deliveryMethodId, basketId, shippingAddress),"Order is created successfully");


        }
        [HttpGet("GetOrdersForUser")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            return SendSuccessResponse(await _orderService.orderService.GetOrdersForUserAsync(email));
        }
        [HttpGet("GetOrderDetails")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrderDetails(Guid orderId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            return SendSuccessResponse(await _orderService.orderService.GetOrderByIdAsync(orderId, email));
        }
        [HttpGet("GetUserAdresses")]
        public async Task<ActionResult> GetUserAdresses()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            return SendSuccessResponse(await _orderService.orderService.GetUserAddressesAsync(email));

        }
        [HttpPost("AddAddress")]
        public async Task<ActionResult> AddAddress(AddressBookDto addressDto) {
            var email = User.FindFirstValue(ClaimTypes.Email);

            return SendSuccessResponse(await _orderService.orderService.AddUserAddressAsync(email, addressDto));
        }


    }
}
