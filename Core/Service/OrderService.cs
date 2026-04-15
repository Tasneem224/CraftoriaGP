using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.CartModule;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using DomainLayer.Models.Order;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction;
using Shared.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Service
{
    public class OrderService(UserManager<ApplicationUser> _userManager,ICartRepository _cartRepository,IUnitOfWork _unitOfWork,ICartService _cartService) : IOrderService
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
            public async Task<IReadOnlyList<OrderToReturnDto>> GetOrdersForUserAsync(string userEmail)
            {
            
                var orders = await _unitOfWork.Orders.GetOrdersForUserWithItemsAsync(userEmail);

                return orders.Select(order => MapOrderToDto(order)).ToList();
            }
        public async Task<IEnumerable<AddressBookDto>> GetUserAddressesAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) throw new Exception("User not found");

            var query = _unitOfWork.GetRepository<Address_Book, Guid>().GetAllQueryable();

            return await query
                .Where(a => a.AppUserId == user.Id)
                .Select(a => new AddressBookDto
                {
                    FullName = a.FullName,
                    StreetDetails = a.StreetDetails,
                    PhoneNumber = a.PhoneNumber,
                    City = a.City,
                    Region = a.Region
                })
                .ToListAsync();
        }
        public async Task<AddressBookDto> AddUserAddressAsync(string userEmail, AddressBookDto addressDto)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) throw new Exception("User not found");

            var newAddressEntry = new Address_Book
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                FullName = addressDto.FullName,
                StreetDetails = addressDto.StreetDetails,
                PhoneNumber = addressDto.PhoneNumber,
                City = addressDto.City,
                Region = addressDto.Region,
                State = "N/A"
            };

            await _unitOfWork.GetRepository<Address_Book, Guid>().AddAsync(newAddressEntry);
            var result = await _unitOfWork.SaveChanges();

            if (result <= 0) throw new Exception("Failed to save address to address book");

            return addressDto;
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
        private OrderToReturnDto MapOrderToDto(Order order)
        {
            return new OrderToReturnDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                UserEmail = order.UserEmail,
                DeliveryMethod = order.DeliveryMethod?.ShortName ?? "N/A",
                OrderPaymentStatus = order.orderStatus.ToString(), 
                ShippingPrice = order.DeliveryMethod?.Price ?? 0,
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
    }
}
