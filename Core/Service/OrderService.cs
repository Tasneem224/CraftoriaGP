using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.CartModule;
using DomainLayer.Models.Items;
using DomainLayer.Models.Order;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction;
using Shared.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class OrderService(ICartRepository _cartRepository,IUnitOfWork _unitOfWork,ICartService _cartService) : IOrderService
    {
        public async Task<OrderToReturnDto?> CreateOrderAsync(string userEmail, int deliveryMethodId, string basketId, AddressBookDto shippingAddress)
        {
            var basket = await _cartRepository.GetAsync<CustomerCart>(basketId);
            if (basket == null) throw new CartNotFoundException(basketId);

            int maxRetries = 3;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var orderItems = await PrepareOrderItemsAsync(basket);

                    var order = await BuildOrderAsync(userEmail, deliveryMethodId, shippingAddress, orderItems);

                    await _unitOfWork.GetRepository<Order, Guid>().AddAsync(order);
                    var result = await _unitOfWork.SaveChanges();

                    if (result > 0)
                    {
                        await _cartRepository.DeleteAsync(basketId);
                        return await GetOrderByIdAsync(order.Id, userEmail);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (i == maxRetries - 1) throw new Exception("Something went wront ,Please try again");
                    continue;
                }
            }
            return null;
        }
        private async Task<List<OrderItem>> PrepareOrderItemsAsync(CustomerCart basket)
        {
            var orderItems = new List<OrderItem>();

            foreach (var item in basket.cartItems)
            {
                var productItem = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);
                if (productItem == null) continue;

                if (productItem.Quantity < item.Quantity)
                    throw new Exception($"المنتج {productItem.NameEn} خلص!");

                productItem.Quantity -= item.Quantity;
                _unitOfWork.GetRepository<Product, int>().Update(productItem);

                orderItems.Add(new OrderItem
                {
                    Item = new ItemInOrderItem
                    {
                        ItemId = productItem.Id,
                        ItemName = productItem.NameEn,
                        ItemPictureUrl = productItem.ImageUrl
                    },
                    Price = productItem.Price,
                    Quantity = item.Quantity
                });
            }
            return orderItems;
        }
        private async Task<Order> BuildOrderAsync(string email, int deliveryId, AddressBookDto addressDto, List<OrderItem> items)
        {
            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(deliveryId);
            var subtotal = items.Sum(i => i.Price * i.Quantity);

            return new Order
            {
                UserEmail = email,
                ShippingAddress = new Address
                {
                    FullName = addressDto.FullName,
                    StreetDetails = addressDto.StreetDetails,
                    City = addressDto.City,
                    Region = addressDto.Region,
                    PhoneNumber = addressDto.PhoneNumber
                },
                DeliveryMethod = deliveryMethod,
                OrderItems = items,
                Subtotal = subtotal,
                DeliveryMethodId = deliveryMethod.Id
            };
        }
        public async Task<OrderToReturnDto?> GetOrderByIdAsync(Guid id, string userEmail)
        {
           var order =await _unitOfWork.Orders.GetOrderByIdWithItemsAsync(id);
            if (order == null)
                throw new ItemNotFound("Order not found");
            if(order.UserEmail != userEmail)
                throw new UnauthorizedAccessException("You are not authorized to access this order");

            return new OrderToReturnDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                UserEmail = order.UserEmail,
                DeliveryMethod =order.DeliveryMethod.ShortName,
                OrderPaymentStatus = order.orderPaymentStatus.ToString(),
                ShippingPrice = order.DeliveryMethod.Price,
                Status = order.orderStatus.ToString(),
                Subtotal = order.Subtotal,
                Total = order.GetTotal(),
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.Item.ItemId,
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
