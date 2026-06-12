using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [Authorize] // حماية كاملة: لازم يكون المستخدم باعت الـ JWT Token
    public class NotificationsController: BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// جلب كل الإشعارات الخاصة بالمستخدم الحالي (مرتبة من الأحدث للأقدم)
        /// GET: api/notifications/my-notifications
        /// </summary>
        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            // سحب الـ UserId من التوكن (الادعاءات)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not identified." });

            var notifications = await _notificationService.GetMyNotificationsAsync(userId);

            return Ok(notifications);
        }

        /// <summary>
        /// تحديث حالة إشعار معين ليصبح "مقروء" (عشان النقطة تنطفي في الـ UI)
        /// PUT: api/notifications/mark-as-read/5
        /// </summary>
        [HttpPut("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _notificationService.MarkAsReadAsync(id, userId);

            if (!result)
                return NotFound(new { message = "Notification not found or access denied." });

            return Ok(new { message = "Notification marked as read." });
        }

        /// <summary>
        /// إرسال إشعار يدوي لمستخدم معين (خاص بالأدمن فقط)
        /// POST: api/notifications/send
        /// </summary>
        [Authorize(Roles = "Admin")] // حماية: الأدمن فقط هو من يستطيع ضرب هذه الـ Endpoint
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _notificationService.SendNotificationAsync(request);

            return Ok(new { message = "Notification sent successfully." });
        }

    }
}
