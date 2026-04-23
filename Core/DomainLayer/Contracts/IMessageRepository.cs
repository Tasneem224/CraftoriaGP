using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models.Chat;
using global::DomainLayer.Models.Chat;

namespace DomainLayer.Contracts
{
   

    
        /// <summary>
        /// Extends IGenericRepository with chat-specific query operations that are
        /// too complex (multi-table JOINs, grouping, pagination) for the generic repo.
        /// </summary>
        public interface IMessageRepository : IGenericRepository<Message, int>
        {
            // ── Conversation ─────────────────────────────────────────────────────────

            /// <summary>
            /// Returns a paginated, chronological list of messages shared between
            /// exactly two users — regardless of who was sender or receiver.
            /// </summary>
            Task<IEnumerable<Message>> GetConversationAsync(
                string userId1,
                string userId2,
                int pageNumber,
                int pageSize);

            /// <summary>Total number of messages in the conversation (for pagination meta).</summary>
            Task<int> GetConversationCountAsync(string userId1, string userId2);

            // ── Inbox ─────────────────────────────────────────────────────────────────

            /// <summary>
            /// Returns the most recent message from every distinct conversation the
            /// current user participates in (inbox / home screen).
            /// Includes navigation properties for Sender and Receiver so the service
            /// can build InboxItemDtos without extra round-trips.
            /// </summary>
            Task<IEnumerable<Message>> GetInboxAsync(string userId);

            // ── Read Receipts ─────────────────────────────────────────────────────────

            /// <summary>
            /// Marks a single message as read and sets ReadAt to UtcNow.
            /// Returns the updated message (needed to fire SignalR notification with
            /// the exact read timestamp) or null if not found / not authorised.
            /// </summary>
            Task<Message?> MarkMessageAsReadAsync(int messageId, string readerId);

            /// <summary>
            /// Bulk-marks every unread message sent BY otherUserId TO currentUserId as
            /// read. Used when the user opens a conversation.
            /// Returns the count of rows updated.
            /// </summary>
            Task<int> MarkConversationAsReadAsync(string currentUserId, string otherUserId);

            // ── Counts ───────────────────────────────────────────────────────────────

            /// <summary>Total unread messages received by the specified user.</summary>
            Task<int> GetTotalUnreadCountAsync(string userId);

            /// <summary>Unread messages per sender — useful for badge counts.</summary>
            Task<Dictionary<string, int>> GetUnreadCountPerSenderAsync(string receiverId);

            // ── Single Message ────────────────────────────────────────────────────────

            /// <summary>
            /// Retrieves a single message with Sender/Receiver navigation loaded.
            /// Returns null if the message does not exist or does not belong to userId.
            /// </summary>
            Task<Message?> GetMessageByIdWithUsersAsync(int messageId, string userId);

            // ── Soft Delete ───────────────────────────────────────────────────────────

            /// <summary>
            /// Soft-deletes a message (sets IsDeleted = true).
            /// Only the original sender may delete their own message.
            /// </summary>
            Task<bool> SoftDeleteMessageAsync(int messageId, string requestingUserId);
        }
    }

