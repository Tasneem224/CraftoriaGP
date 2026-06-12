using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DomainLayer.Contracts; // عشان يشوف الـ IUnitOfWork
using DomainLayer.Models.Notifications;
using ServiceAbstraction;
using Shared.Notifications;

namespace Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRealtimeNotificationService _realtimeService;

        public NotificationService(IUnitOfWork unitOfWork, IRealtimeNotificationService realtimeService)
        {
            _unitOfWork = unitOfWork;
            _realtimeService = realtimeService;
        }

        // 1. إرسال وحفظ الإشعار باستخدام الـ Generic Repository والـ UoW
        public async Task SendNotificationAsync(SendNotificationDto request)
        {
            // أ - سحب الـ Generic Repository الخاص بالـ Notification
            var repo = _unitOfWork.GetRepository<Notification, int>();

            var notification = new Notification
            {
                UserId = request.UserId,
                TitleEn = request.TitleEn,
                TitleAr = request.TitleAr,
                MessageEn = request.MessageEn,
                MessageAr = request.MessageAr,
                Type = (NotificationType)request.Type,
                RelatedId = request.RelatedId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            // ب - الحفظ عن طريق الريبو والـ Unit of Work
            await repo.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync(); // هنا الـ UoW بتعمل التعديل في الداتابيز

            // ج - تحويل البيانات لـ Response DTO وإرسالها لحظياً
            var responseDto = new NotificationResponseDto
            {
                Id = notification.Id,
                TitleEn = notification.TitleEn,
                TitleAr = notification.TitleAr,
                MessageEn = notification.MessageEn,
                MessageAr = notification.MessageAr,
                Type = notification.Type.ToString(),
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                RelatedId = notification.RelatedId
            };

            await _realtimeService.SendNotificationToUserAsync(request.UserId, responseDto);
        }

        // 2. جلب إشعارات المستخدم عن طريق الـ Queryable الخاص بالـ Generic Repo
        public async Task<List<NotificationResponseDto>> GetMyNotificationsAsync(string currentUserId)
        {
            var repo = _unitOfWork.GetRepository<Notification, int>();

            return await repo.GetAllQueryable()
                .Where(n => n.UserId == currentUserId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDto
                {
                    Id = n.Id,
                    TitleEn = n.TitleEn,
                    TitleAr = n.TitleAr,
                    MessageEn = n.MessageEn,
                    MessageAr = n.MessageAr,
                    Type = n.Type.ToString(),
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    RelatedId = n.RelatedId
                })
                .ToListAsync();
        }

        // 3. تحديث حالة الإشعار باستخدام الـ Generic Repo
        public async Task<bool> MarkAsReadAsync(int notificationId, string currentUserId)
        {
            var repo = _unitOfWork.GetRepository<Notification, int>();

            var notification = await repo.GetAllQueryable()
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == currentUserId);

            if (notification == null) return false;

            notification.IsRead = true;

            repo.Update(notification); // ميثود الـ Update اللي في الـ Generic Repo بتاعك
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}