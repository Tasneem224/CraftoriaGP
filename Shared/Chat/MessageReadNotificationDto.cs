using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Chat
{
    /// <summary>
    /// Pushed over SignalR to the ORIGINAL SENDER when the receiver reads a message.
    /// Allows the sender's UI to display a "seen" / double-tick indicator.
    /// </summary>
    public class MessageReadNotificationDto
    {
        public int MessageId { get; set; }
        public string ReadByUserId { get; set; } = string.Empty;
        public DateTime ReadAt { get; set; }
    }
}
