using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Notifications
{
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string MessageEn { get; set; }
        public string MessageAr { get; set; }
        public string Type { get; set; } // هنباصيها كـ string (Order, Chat, AppUpdate) عشان الفلاتر يتعامل معاها أسهل
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RelatedId { get; set; }
    }
}
