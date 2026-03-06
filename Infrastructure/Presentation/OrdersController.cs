using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        public OrdersController(IServiceManager serviceManager) =>
            _orderService = serviceManager.orderService;

        [HttpPost("CreateOrder")]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(string userEmail, int deliveryMethodId, string basketId, AddressBookDto shippingAddress)
        {
            return SendSuccessResponse(await _orderService.CreateOrderAsync(userEmail, deliveryMethodId, basketId, shippingAddress),"Order is created successfully");


        }
        [HttpGet("GetOrdersForUser")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser(string userEmail)
        {
            return SendSuccessResponse(await _orderService.GetOrdersForUserAsync(userEmail));
        }
        [HttpGet("GetOrderDetails")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrderDetails(Guid orderId, string userEmail)
        {
            return SendSuccessResponse(await _orderService.GetOrderByIdAsync(orderId,userEmail));
        }


    }
}
