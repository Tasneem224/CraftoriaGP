using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.CartModule;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using DomainLayer.Models.Notifications;
using DomainLayer.Models.Order;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Factory;
using ServiceAbstraction;
using Shared.Notifications;
using Shared.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Service
{
    public class OrderService(PaymentServiceFactory _paymentFactory,UserManager<ApplicationUser> _userManager,ICartRepository _cartRepository,IUnitOfWork _unitOfWork,ICartService _cartService, INotificationService _notificationService) : IOrderService
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
                    var result = await _unitOfWork.SaveChangesAsync();

                    if (result > 0)
                    {
                        await _cartRepository.DeleteAsync(basketId);

                        // 🔔 [إشعار 1]: إرسال إشعار للمستخدم بأن الطلب تم إنشاؤه بنجاح وgاري الدفع
                        var user = await _userManager.FindByEmailAsync(userEmail);
                        if (user != null)
                        {
                            await _notificationService.SendNotificationAsync(new SendNotificationDto
                            {
                                UserId = user.Id,
                                TitleEn = "Order Placed Successfully",
                                TitleAr = "تم تسجيل طلبك بنجاح",
                                MessageEn = $"Your order has been created. Total: {order.GetTotal()} EGP. Please proceed to payment.",
                                MessageAr = $"تم إنشاء طلبك بنجاح. الإجمالي: {order.GetTotal()} ج.م. يمكنك الآن إتمام عملية الدفع.",
                                Type = (NotificationType2)NotificationType.Order,
                                RelatedId = order.Id.ToString() // تحويل الـ Guid لـ string
                            });
                        }

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
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0) throw new Exception("Failed to save address to address book");

            return addressDto;
        }
        public async Task<string> CreatePaymentAsync(Guid orderId, string method)
        {
            // 1. جلب الأوردر (الـ Service هي اللي بتكلم الـ UnitOfWork)
            var order = await _unitOfWork.Orders.GetOrderByIdWithItemsAsync(orderId);
            if (order == null) throw new Exception("Order not found");

            // 2. الـ Mapping (تحويل الـ Entity لـ DTO)
            var orderToPaymentDto = new OrderToPaymentDto
            {
                OrderId = order.Id,
                UserEmail = order.UserEmail,
                TotalAmount = order.GetTotal(),
                FullName = order.ShippingAddress.FullName,
                Phone = order.ShippingAddress.PhoneNumber,
                City = order.ShippingAddress.City,
                Street = order.ShippingAddress.StreetDetails,
                Items = order.OrderItems.Select(oi => new OrderItemPaymentDto
                {
                    ProductName = oi.Item.ItemName,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };

            // 3. استخدام الـ Factory (السيرفيس هي اللي بتستخدمه)
            var paymentService = _paymentFactory.GetPaymentService(method);

            // 4. تنفيذ الدفع
            return await paymentService.CreatePaymentSession(orderToPaymentDto);
        }
        public async Task UpdateOrderStatusAndWallets(string paymentIntentId)
        {
            // 1. جلب الأوردر بالـ PaymentIntentId (لازم الـ Repo يكون بيدعم الميثود دي)
            var order = await _unitOfWork.Orders.GetOrderByPaymentIntentIdAsync(paymentIntentId);

            if (order == null) throw new Exception("Order not found for this PaymentIntent");

            // 2. تحديث حالة الأوردر (الدفع تم بنجاح)
            order.orderPaymentStatus = OrderPaymentStatus.Received;
            order.orderStatus = OrderStatus.Confirmed;

            // 3. توزيع الأرباح على محافظ التجار (Looping through order items)
            foreach (var item in order.OrderItems)
            {
                // بنجيب ريبوزيتوري المحفظة
                var walletRepo = _unitOfWork.GetRepository<VendorWallet, int>();

                // بندور على محفظة التاجر (Seller) صاحب المنتج ده
                var wallet = await walletRepo.GetAllQueryable()
                    .FirstOrDefaultAsync(w => w.VendorId == item.VendorId);

                if (wallet == null)
                {
                    // لو التاجر لسه ملوش محفظة في السيستم بنكريت واحدة
                    wallet = new VendorWallet
                    {
                        VendorId = item.VendorId,
                        Balance = 0,
                        LastUpdated = DateTime.UtcNow
                    };
                    await walletRepo.AddAsync(wallet);
                }

                // الحتة السحرية: بنزود رصيد التاجر بالـ 70% اللي حسبناها وسيفناها وقت الأوردر
                wallet.Balance += item.VendorNetEarnings;
                wallet.LastUpdated = DateTime.UtcNow;

                walletRepo.Update(wallet);
            }

            // 4. حفظ كل التغييرات (الأوردر والمحافظ) في Transaction واحدة
            await _unitOfWork.SaveChangesAsync();

            // 🔔 [إشعار 2]: إرسال إشعار فوري للمستخدم بعد ما السيف تم والمحفظة اتحدثت إن الدفع نجح والأوردر اتأكد!
            var user = await _userManager.FindByEmailAsync(order.UserEmail);
            if (user != null)
            {
                await _notificationService.SendNotificationAsync(new SendNotificationDto
                {
                    UserId = user.Id,
                    TitleEn = "Payment Confirmed! 🎉",
                    TitleAr = "تم تأكيد الدفع بنجاح! 🎉",
                    MessageEn = $"Payment for order #{order.Id} was received. Your order is now confirmed.",
                    MessageAr = $"تم استلام دفع الطلب رقم #{order.Id} بنجاح، وطلبك الآن قيد التجهيز.",
                    Type = (NotificationType2)NotificationType.Order,
                    RelatedId = order.Id.ToString()
                });
            }
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

                var lineTotal = productItem.Price * item.Quantity;
                var commission = lineTotal * 0.30m;
                var netEarnings = lineTotal - commission;

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
                    Quantity = item.Quantity,
                    VendorId = productItem.SellerId, 
                    AppCommission = commission,      
                    VendorNetEarnings = netEarnings  
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
                CommissionAmount = items.Sum(i => i.AppCommission), // مجموع الـ 30% من كل المنتجات
                VendorNetEarnings = items.Sum(i => i.VendorNetEarnings), // مجموع الـ 70% اللي هيروح للتجار
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
