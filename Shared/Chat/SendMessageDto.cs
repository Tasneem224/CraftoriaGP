using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Chat
{
    /// <summary>
    /// Payload the client sends when posting a new message.
    /// NOTE: SenderId is intentionally absent — it is ALWAYS extracted from the
    /// authenticated user's JWT claims inside the controller to prevent spoofing.
    /// </summary>
    public class SendMessageDto
    {
        /// <summary>ApplicationUser.Id of the intended recipient.</summary>
        public string ReceiverId { get; set; } = string.Empty;

        /// <summary>Text content of the message (1–2000 chars).</summary>
        public string Content { get; set; } = string.Empty;
    }
}
