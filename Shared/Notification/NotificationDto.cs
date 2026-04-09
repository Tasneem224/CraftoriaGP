using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        // لو عندك نوع للإشعار (مثلا: رسالة جديدة، طلب جديد) ممكن تضيفيه هنا
        public NotificationTypeDto type { get; set; } 
    }
}
