using DomainLayer.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IOrderRepository:IGenericRepository<Order, Guid>
    {
        Task<Order?> GetOrderByPaymentIntentIdAsync(string paymentIntentId);
        Task<Order?> GetOrderByIdWithItemsAsync(Guid id);
        Task<IEnumerable<Order>> GetOrdersForUserWithItemsAsync(string email);
    }
}
