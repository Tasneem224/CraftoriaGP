using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Order;
using ServiceAbstraction;
using Shared.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class OrderService(IUnitOfWork _unitOfWork,ICartService _cartService) : IOrderService
    {
        public Task<OrderToReturnDto?> CreateOrderAsync(string userEmail, int deliveryMethodId, string basketId, AddressBookDto shippingAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderToReturnDto?> GetOrderByIdAsync(Guid id, string userEmail)
        {
           var order =await _unitOfWork.GetRepository<Order,Guid>().GetByIdAsync(id);
            if (order == null)
                throw new ItemNotFound("Order not found");
            if(order.UserEmail != userEmail)
                throw new UnauthorizedAccessException("You are not authorized to access this order");
            return new OrderToReturnDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                UserEmail = order.UserEmail,
                DeliveryMethod = ,
                OrderPaymentStatus = order.orderPaymentStatus,
                ShippingPrice = order.ShippingPrice,
                Status = order.Status,
                Subtotal = order.Subtotal,
                Total = order.GetTotal(),
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.Id,
                    ProductName = oi.Item.ItemName,
                    PictureUrl = oi.Item.ItemPictureUrl,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }

        public Task<IReadOnlyList<OrderToReturnDto>> GetOrdersForUserAsync(string userEmail)
        {
            throw new NotImplementedException();
        }
    }
}
