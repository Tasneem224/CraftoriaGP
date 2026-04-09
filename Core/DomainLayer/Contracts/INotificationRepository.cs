using DomainLayer.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    // ضيفنا , int هنا عشان نحدد نوع المفتاح
    public interface INotificationRepository : IGenericRepository<Notification, int>
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task ClearAllAsync(string userId);
    }
}
