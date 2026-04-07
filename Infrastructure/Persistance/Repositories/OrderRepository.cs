using DomainLayer.Contracts;
using DomainLayer.Models.Order;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class OrderRepository(StoreDbContext _Context) : GenericRepository<Order, Guid>(_Context), IOrderRepository
    {

        public async Task<Order?> GetOrderByIdWithItemsAsync(Guid id)
        {
            return await _Context.Orders
                .Include(o => o.OrderItems)    
                .Include(o => o.DeliveryMethod)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<IEnumerable<Order>> GetOrdersForUserWithItemsAsync(string email)
        {
            return await _Context.Orders
                .Where(o => o.UserEmail == email)
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryMethod)
                .OrderByDescending(o => o.OrderDate) // عشان الأحدث يظهر الأول
                .ToListAsync();
        }
        

    }
}
