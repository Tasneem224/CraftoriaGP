using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Chat;

namespace ServiceAbstraction
{
    

    
        /// <summary>
        /// Defines all chat-message business operations.
        /// Implementations live in the Service project and must not reference
        /// Presentation or CraftoriaApp.
        /// </summary>
        public interface IMessageService
        {
            // ── Core Features ────────────────────────────────────────────────────────

            /// <summary>
            /// Persists a message to the database and then fires a real-time
            /// SignalR event to the receiver via IRealtimeNotificationService.
            /// </summary>
            Task<MessageResponseDto> SendMessageAsync(string senderId, SendMessageDto dto);

            /// <summary>
            /// Returns a paginated, chronological list of messages between the current
            /// user and another user.
            /// </summary>
            Task<PagedResultDto<MessageResponseDto>> GetConversationAsync(
                string currentUserId,
                string otherUserId,
                ConversationRequestDto request);

            /// <summary>
            /// Returns the latest message from every conversation the user is part of
            /// (the inbox / home screen).
            /// </summary>
            Task<IEnumerable<InboxItemDto>> GetInboxAsync(string currentUserId);

            /// <summary>
            /// Marks a single message as read and fires a SignalR read-receipt to the
            /// original sender.
            /// </summary>
            Task<bool> MarkMessageAsReadAsync(int messageId, string currentUserId);

            // ── Bonus / Future-Proof Endpoints ───────────────────────────────────────

            /// <summary>
            /// Marks ALL unread messages in a conversation as read in one call.
            /// Useful when the user opens a conversation screen.
            /// </summary>
            Task<int> MarkConversationAsReadAsync(string currentUserId, string otherUserId);

            /// <summary>Total unread-message count (for global badge / notification dot).</summary>
            Task<int> GetTotalUnreadCountAsync(string userId);

            /// <summary>Unread-message counts grouped by sender ID (per-conversation badges).</summary>
            Task<Dictionary<string, int>> GetUnreadCountPerSenderAsync(string userId);

            /// <summary>
            /// Retrieves a single message if it belongs to the requesting user.
            /// </summary>
            Task<MessageResponseDto?> GetMessageByIdAsync(int messageId, string currentUserId);

            /// <summary>
            /// Soft-deletes a message.  Only the original sender may delete.
            /// </summary>
            Task<bool> DeleteMessageAsync(int messageId, string currentUserId);
        }
    }

