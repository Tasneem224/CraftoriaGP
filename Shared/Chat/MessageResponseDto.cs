using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Chat
{
    public class MessageResponseDto
    {
        public int Id { get; set; }

        // ── Sender ──────────────────────────────────────────────────────────────
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string? SenderAvatar { get; set; }

        // ── Receiver ─────────────────────────────────────────────────────────────
        public string ReceiverId { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;

        // ── Content ──────────────────────────────────────────────────────────────
        public string Content { get; set; } = string.Empty;

        // ── Status ───────────────────────────────────────────────────────────────
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime SentAt { get; set; }   // maps to CreatedAt

        /// <summary>True when the currently authenticated user is the sender.</summary>
        public bool IsOwnMessage { get; set; }
    }
}
