using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Notifications
{
    public class SendNotificationDto
    {
        public string UserId { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string MessageEn { get; set; }
        public string MessageAr { get; set; }
        public NotificationType2 Type { get; set; }
        public int? RelatedId { get; set; }
    }
}
