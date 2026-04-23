using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Contracts;
using DomainLayer.Models.Chat;
using ServiceAbstraction;
using Shared.Chat;

namespace Service
{
   

    
        /// <summary>
        /// Orchestrates all chat-message business logic.
        ///
        /// Deliberately has NO reference to SignalR, IHubContext, or any Presentation
        /// type. Real-time events are triggered through IRealtimeNotificationService
        /// (the bridge interface in ServiceAbstraction).
        /// </summary>
        public class MessageService : IMessageService
        {
            private readonly IUnitOfWork _uow;
            private readonly IRealtimeNotificationService _realtime;

            public MessageService(
                IUnitOfWork uow,
                IRealtimeNotificationService realtime)
            {
                _uow = uow;
                _realtime = realtime;
            }

            // ════════════════════════════════════════════════════════════════════════
            // 1. SEND MESSAGE
            // ════════════════════════════════════════════════════════════════════════

            /// <inheritdoc/>
            public async Task<MessageResponseDto> SendMessageAsync(
                string senderId,
                SendMessageDto dto)
            {
                // ── Guard ────────────────────────────────────────────────────────────
                if (string.IsNullOrWhiteSpace(senderId))
                    throw new UnauthorizedAccessException("Sender identity could not be determined.");

                if (string.IsNullOrWhiteSpace(dto.ReceiverId))
                    throw new ArgumentException("ReceiverId is required.", nameof(dto));

                if (string.IsNullOrWhiteSpace(dto.Content))
                    throw new ArgumentException("Message content cannot be empty.", nameof(dto));

                if (senderId == dto.ReceiverId)
                    throw new InvalidOperationException("A user cannot send a message to themselves.");

                // ── Build the entity ──────────────────────────────────────────────────
                // SenderId is injected by the SERVICE (from JWT claims forwarded by the
                // controller), never taken from the client payload → fixes the FK bug.
                var now = DateTime.UtcNow;

                var message = new Message
                {
                    SenderId = senderId,          // ← safe: comes from JWT
                    ReceiverId = dto.ReceiverId,
                    Content = dto.Content.Trim(),
                    IsRead = false,
                    CreatedAt = now,
                    CreatedBy = senderId,          // audit trail
                    UpdatedAt = now,
                    IsDeleted = false
                };

                // ── Persist ───────────────────────────────────────────────────────────
                await _uow.Messages.AddAsync(message);
                await _uow.SaveChanges();

                // Reload with navigation properties so the DTO has names/avatars
                var saved = await _uow.Messages.GetMessageByIdWithUsersAsync(message.Id, senderId)
                            ?? throw new Exception("Failed to retrieve saved message.");

                var responseDto = MapToResponseDto(saved, senderId);

                // ── Real-time: push to receiver ───────────────────────────────────────
                // If the receiver is not connected the call is a no-op (SignalR handles it).
                await _realtime.SendMessageToUserAsync(dto.ReceiverId, responseDto);

                return responseDto;
            }

            // ════════════════════════════════════════════════════════════════════════
            // 2. GET CONVERSATION (paginated)
            // ════════════════════════════════════════════════════════════════════════

            /// <inheritdoc/>
            public async Task<PagedResultDto<MessageResponseDto>> GetConversationAsync(
                string currentUserId,
                string otherUserId,
                ConversationRequestDto request)
            {
                var messages = await _uow.Messages.GetConversationAsync(
                    currentUserId, otherUserId, request.PageNumber, request.PageSize);

                var total = await _uow.Messages.GetConversationCountAsync(
                    currentUserId, otherUserId);

                return new PagedResultDto<MessageResponseDto>
                {
                    Data = messages.Select(m => MapToResponseDto(m, currentUserId)),
                    TotalCount = total,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            // ════════════════════════════════════════════════════════════════════════
            // 3. GET INBOX
            // ════════════════════════════════════════════════════════════════════════

            /// <inheritdoc/>
            public async Task<IEnumerable<InboxItemDto>> GetInboxAsync(string currentUserId)
            {
                var latestMessages = await _uow.Messages.GetInboxAsync(currentUserId);

                // Fetch per-sender unread counts in a single query for efficiency
                var unreadCounts = await _uow.Messages
                    .GetUnreadCountPerSenderAsync(currentUserId);

                var inbox = latestMessages.Select(m =>
                {
                    // "Other user" is whoever is NOT the current user in this message
                    var isOwnMessage = m.SenderId == currentUserId;
                    var otherUserId = isOwnMessage ? m.ReceiverId : m.SenderId;
                    var otherUser = isOwnMessage ? m.Receiver : m.Sender;

                    unreadCounts.TryGetValue(otherUserId, out var unread);

                    return new InboxItemDto
                    {
                        OtherUserId = otherUserId,
                        OtherUserName = otherUser?.UserName ?? "Unknown",
                        OtherAvatar = otherUser?.ProfileImage,   // adjust to your property name
                        LastMessage = m.Content,
                        LastMessageTime = m.CreatedAt,
                        IsLastMessageMine = isOwnMessage,
                        UnreadCount = unread
                    };
                });

                // Sort so most recent conversation is first
                return inbox.OrderByDescending(i => i.LastMessageTime);
            }

            // ════════════════════════════════════════════════════════════════════════
            // 4. MARK SINGLE MESSAGE AS READ
            // ════════════════════════════════════════════════════════════════════════

            /// <inheritdoc/>
            public async Task<bool> MarkMessageAsReadAsync(int messageId, string currentUserId)
            {
                var updated = await _uow.Messages.MarkMessageAsReadAsync(messageId, currentUserId);

                if (updated is null) return false;

                await _uow.SaveChanges();

                // ── Real-time: notify original sender ─────────────────────────────────
                var notification = new MessageReadNotificationDto
                {
                    MessageId = updated.Id,
                    ReadByUserId = currentUserId,
                    ReadAt = updated.ReadAt!.Value
                };

                await _realtime.NotifyMessageReadAsync(updated.SenderId, notification);

                return true;
            }

            // ════════════════════════════════════════════════════════════════════════
            // 5. MARK ENTIRE CONVERSATION AS READ
            // ════════════════════════════════════════════════════════════════════════

            /// <inheritdoc/>
            public async Task<int> MarkConversationAsReadAsync(
                string currentUserId,
                string otherUserId)
            {
                var count = await _uow.Messages
                    .MarkConversationAsReadAsync(currentUserId, otherUserId);

                if (count > 0) await _uow.SaveChanges();

                return count;
            }

            // ════════════════════════════════════════════════════════════════════════
            // 6. UNREAD COUNTS
            // ════════════════════════════════════════════════════════════════════════

            /// <inheritdoc/>
            public async Task<int> GetTotalUnreadCountAsync(string userId)
                => await _uow.Messages.GetTotalUnreadCountAsync(userId);

            /// <inheritdoc/>
            public async Task<Dictionary<string, int>> GetUnreadCountPerSenderAsync(string userId)
                => await _uow.Messages.GetUnreadCountPerSenderAsync(userId);

            // ════════════════════════════════════════════════════════════════════════
            // 7. GET SINGLE MESSAGE
            // ════════════════════════════════════════════════════════════════════════

            /// <inheritdoc/>
            public async Task<MessageResponseDto?> GetMessageByIdAsync(
                int messageId,
                string currentUserId)
            {
                var message = await _uow.Messages
                    .GetMessageByIdWithUsersAsync(messageId, currentUserId);

                return message is null ? null : MapToResponseDto(message, currentUserId);
            }

            // ════════════════════════════════════════════════════════════════════════
            // 8. SOFT DELETE
            // ════════════════════════════════════════════════════════════════════════

            /// <inheritdoc/>
            public async Task<bool> DeleteMessageAsync(int messageId, string currentUserId)
            {
                var deleted = await _uow.Messages
                    .SoftDeleteMessageAsync(messageId, currentUserId);

                if (deleted) await _uow.SaveChanges();

                return deleted;
            }

            // ════════════════════════════════════════════════════════════════════════
            // PRIVATE HELPERS
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Maps a Message entity (with navigation properties loaded) to a
            /// MessageResponseDto.  Adjust property names to match your ApplicationUser.
            /// </summary>
            private static MessageResponseDto MapToResponseDto(Message m, string currentUserId)
            {
                return new MessageResponseDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderName = m.Sender?.UserName ?? string.Empty,
                    SenderAvatar = m.Sender?.ProfileImage,  // adjust to your property
                    ReceiverId = m.ReceiverId,
                    ReceiverName = m.Receiver?.UserName ?? string.Empty,
                    Content = m.Content,
                    IsRead = m.IsRead,
                    ReadAt = m.ReadAt,
                    SentAt = m.CreatedAt,
                    IsOwnMessage = m.SenderId == currentUserId
                };
            }
        }
    }

