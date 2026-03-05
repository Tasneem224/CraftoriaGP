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
        Task<OrderToReturnDto?> CreateOrderAsync(string userEmail, int deliveryMethodId, string basketId, AddressBookDto shippingAddress);
        Task<IReadOnlyList<OrderToReturnDto>> GetOrdersForUserAsync(string userEmail);
        Task<OrderToReturnDto?> GetOrderByIdAsync(Guid id, string userEmail);
    }
}
