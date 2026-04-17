using Shared.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IOrderService
    {

        Task UpdateOrderStatusAndWallets(string paymentIntentId);
        Task<string> CreatePaymentAsync(Guid orderId, string method);
        Task<OrderToReturnDto?> CreateOrderAsync(string userEmail, int deliveryMethodId, string basketId, AddressBookDto shippingAddress);
        Task<IReadOnlyList<OrderToReturnDto>> GetOrdersForUserAsync(string userEmail);
        Task<OrderToReturnDto?> GetOrderByIdAsync(Guid id, string userEmail);
        Task<IEnumerable<AddressBookDto>> GetUserAddressesAsync(string userEmail);
        Task<AddressBookDto> AddUserAddressAsync(string userEmail, AddressBookDto addressDto);
    }
}
