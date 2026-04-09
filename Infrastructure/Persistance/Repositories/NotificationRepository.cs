using DomainLayer.Contracts;
using DomainLayer.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class NotificationRepository : GenericRepository<Notification,int>, INotificationRepository
    {
        private readonly StoreDbContext _context;

        public NotificationRepository(StoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
            }
        }

        // مسح كل إشعارات اليوزر مرة واحدة
        public async Task ClearAllAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();

            if (notifications.Any())
            {
                _context.Notifications.RemoveRange(notifications);
                // التعديل هيتحفظ لما ننادي SaveChanges من الـ UnitOfWork
            }
        }
    }
}
