using DomainLayer.Contracts;
using Microsoft.AspNetCore.SignalR;
using ServiceAbstraction;
using Shared.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        // نفس الحركة: بنستخدم Hub العام عشان ميبقاش فيه إيرور في النوع (Generic Type)
        private readonly IHubContext<Hub> _hubContext;

        public NotificationService(IUnitOfWork unitOfWork, IHubContext<Hub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _unitOfWork.Notifications.GetUserNotificationsAsync(userId);
            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Content = n.Body,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead,
                type = (NotificationTypeDto)n.Type // الـ Enum اللي إنتي عملتيه
            });
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await _unitOfWork.Notifications.MarkAsReadAsync(notificationId);
            await _unitOfWork.SaveChanges();
        }

        public async Task DeleteAllNotificationsAsync(string userId)
        {
            await _unitOfWork.Notifications.ClearAllAsync(userId);
            await _unitOfWork.SaveChanges();
        }

        // ميثود مساعدة لو حبيتي تبعتي إشعار من أي مكان في السيستم
        public async Task SendNotificationAsync(string userId, NotificationDto notificationDto)
        {
            // إرسال الإشعار فوري للموبايل
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notificationDto);
        }


    }
}
