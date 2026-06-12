using Shared.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface INotificationService
    {
        // ميثود تستخدمها الـ Services الداخلية لإنشاء وإرسال إشعار
        Task SendNotificationAsync(SendNotificationDto request);

        // ميثود يجيب بيها المستخدم إشعاراته (للـ API)
        Task<List<NotificationResponseDto>> GetMyNotificationsAsync(string currentUserId);

        // ميثود لعمل الإشعار كمقروء عشان النقطة تنطفي في الـ UI
        Task<bool> MarkAsReadAsync(int notificationId, string currentUserId);
    }
}
