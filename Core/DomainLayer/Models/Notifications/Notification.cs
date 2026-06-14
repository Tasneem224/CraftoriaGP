using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Notifications
{
    public class Notification:BaseEntity<int>
    {

        // الـ ID بتاع المستخدم اللي هيستلم الإشعار (صاحب الحساب)
        public string UserId { get; set; }

        // نصوص الإشعار (عربي وإنجليزي) عشان يظهر بلغة المستخدم
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string MessageEn { get; set; }
        public string MessageAr { get; set; }

        // نوع الإشعار بناءً على الـ Enum اللي عملناه فوق
        public NotificationType Type { get; set; }

        // هل المستخدم فتح الجرس وشاف الإشعار؟ (عشان النقطة الزرقاء الملغية في الـ UI)
        public bool IsRead { get; set; } = false;

        // وقت إرسال الإشعار
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ID إضافي اختياري (ممكن يشيل OrderId أو ChatId) عشان الـ Navigation في الموبايل
        public string? RelatedId { get; set; }

    }
}
