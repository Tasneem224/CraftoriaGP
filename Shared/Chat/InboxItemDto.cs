using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Chat
{
    /// <summary>
    /// One row in the inbox list — represents the latest message per conversation,
    /// similar to WhatsApp / Telegram home screen.
    /// </summary>
    public class InboxItemDto
    {
        // ── The "other" user in the conversation ─────────────────────────────────
        public string OtherUserId { get; set; } = string.Empty;
        public string OtherUserName { get; set; } = string.Empty;
        public string? OtherAvatar { get; set; }

        // ── Latest message preview ────────────────────────────────────────────────
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public bool IsLastMessageMine { get; set; }

        // ── Unread badge ──────────────────────────────────────────────────────────
        public int UnreadCount { get; set; }
    }
}
