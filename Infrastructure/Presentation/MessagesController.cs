using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Presentation
{
    

        /// <summary>
        /// REST API for the chat system.
        /// All endpoints require a valid JWT — [Authorize] is at class level.
        ///
        /// FK-BUG FIX
        /// ──────────
        /// SenderId is NEVER read from the request body.  It is extracted from
        /// ClaimTypes.NameIdentifier (the "sub" claim set by your JWT middleware).
        /// This guarantees the ID exists in AspNetUsers, eliminating the FK conflict.
        /// </summary>
        
        [Authorize]
        public class MessagesController : BaseApiController
    {
            private readonly IMessageService _messageService;

            public MessagesController(IMessageService messageService)
            {
                _messageService = messageService;
            }

            // ════════════════════════════════════════════════════════════════════════
            // HELPER — extract current user's Id safely from JWT claims
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Reads the authenticated user's ID from the NameIdentifier claim.
            /// Returns null only if the JWT is somehow missing the claim — in
            /// practice [Authorize] prevents any unauthenticated call from reaching here.
            /// </summary>
            private string? CurrentUserId =>
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ════════════════════════════════════════════════════════════════════════
            // 1. POST /api/messages/send
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Sends a message from the authenticated user to a specified receiver.
            /// The message is persisted to the database AND pushed in real-time to
            /// the receiver via SignalR.
            /// </summary>
            [HttpPost("send")]
            [ProducesResponseType(typeof(MessageResponseDto), 201)]
            [ProducesResponseType(400)]
            [ProducesResponseType(401)]
            public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
            {
                var senderId = CurrentUserId;
                if (senderId is null)
                    return Unauthorized(new { message = "Could not determine sender identity from token." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _messageService.SendMessageAsync(senderId, dto);

                // 201 Created with a link to retrieve the message
                return CreatedAtAction(
                    nameof(GetMessageById),
                    new { messageId = result.Id },
                    result);
            }

            // ════════════════════════════════════════════════════════════════════════
            // 2. GET /api/messages/conversation/{otherUserId}
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Returns a paginated, chronological list of messages between the
            /// current user and the specified other user.
            /// Query params: pageNumber (default 1), pageSize (default 20, max 50).
            /// </summary>
            [HttpGet("conversation/{otherUserId}")]
            [ProducesResponseType(typeof(PagedResultDto<MessageResponseDto>), 200)]
            [ProducesResponseType(401)]
            public async Task<IActionResult> GetConversation(
                string otherUserId,
                [FromQuery] ConversationRequestDto request)
            {
                var currentUserId = CurrentUserId;
                if (currentUserId is null) return Unauthorized();

                var result = await _messageService.GetConversationAsync(
                    currentUserId, otherUserId, request);

                return Ok(result);
            }

            // ════════════════════════════════════════════════════════════════════════
            // 3. GET /api/messages/inbox
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Returns the inbox: the latest message from each distinct conversation
            /// the current user participates in, with unread counts — mirrors the
            /// WhatsApp/Telegram home screen UX shown in the provided mockups.
            /// </summary>
            [HttpGet("inbox")]
            [ProducesResponseType(typeof(IEnumerable<InboxItemDto>), 200)]
            [ProducesResponseType(401)]
            public async Task<IActionResult> GetInbox()
            {
                var currentUserId = CurrentUserId;
                if (currentUserId is null) return Unauthorized();

                var result = await _messageService.GetInboxAsync(currentUserId);
                return Ok(result);
            }

            // ════════════════════════════════════════════════════════════════════════
            // 4. PUT /api/messages/{messageId}/read
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Marks a single message as read.
            /// The database is updated AND a real-time read-receipt is pushed to the
            /// original sender via SignalR ("MessageRead" event).
            /// Only the RECEIVER of the message can call this — the service enforces it.
            /// </summary>
            [HttpPut("{messageId:int}/read")]
            [ProducesResponseType(204)]
            [ProducesResponseType(400)]
            [ProducesResponseType(401)]
            [ProducesResponseType(404)]
            public async Task<IActionResult> MarkAsRead(int messageId)
            {
                var currentUserId = CurrentUserId;
                if (currentUserId is null) return Unauthorized();

                var success = await _messageService.MarkMessageAsReadAsync(messageId, currentUserId);

                return success ? NoContent() : NotFound(new { message = "Message not found or not addressed to you." });
            }

            // ════════════════════════════════════════════════════════════════════════
            // 5. PUT /api/messages/conversation/{otherUserId}/read-all
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Bulk-marks ALL unread messages from a specific user as read.
            /// Call this when the user opens a conversation screen.
            /// </summary>
            [HttpPut("conversation/{otherUserId}/read-all")]
            [ProducesResponseType(typeof(object), 200)]
            [ProducesResponseType(401)]
            public async Task<IActionResult> MarkConversationAsRead(string otherUserId)
            {
                var currentUserId = CurrentUserId;
                if (currentUserId is null) return Unauthorized();

                var count = await _messageService.MarkConversationAsReadAsync(currentUserId, otherUserId);
                return Ok(new { markedAsRead = count });
            }

            // ════════════════════════════════════════════════════════════════════════
            // 6. GET /api/messages/unread-count
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Returns the total number of unread messages for the current user.
            /// Useful for the global notification badge / dot on the Messages tab icon.
            /// </summary>
            [HttpGet("unread-count")]
            [ProducesResponseType(typeof(object), 200)]
            [ProducesResponseType(401)]
            public async Task<IActionResult> GetUnreadCount()
            {
                var currentUserId = CurrentUserId;
                if (currentUserId is null) return Unauthorized();

                var count = await _messageService.GetTotalUnreadCountAsync(currentUserId);
                return Ok(new { unreadCount = count });
            }

            // ════════════════════════════════════════════════════════════════════════
            // 7. GET /api/messages/unread-count/per-sender
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Returns unread message counts grouped by sender ID.
            /// Used to populate the per-conversation unread badges in the inbox.
            /// </summary>
            [HttpGet("unread-count/per-sender")]
            [ProducesResponseType(typeof(Dictionary<string, int>), 200)]
            [ProducesResponseType(401)]
            public async Task<IActionResult> GetUnreadCountPerSender()
            {
                var currentUserId = CurrentUserId;
                if (currentUserId is null) return Unauthorized();

                var result = await _messageService.GetUnreadCountPerSenderAsync(currentUserId);
                return Ok(result);
            }

            // ════════════════════════════════════════════════════════════════════════
            // 8. GET /api/messages/{messageId}
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Retrieves a single message by ID.
            /// Returns 404 if the message does not belong to the current user.
            /// </summary>
            [HttpGet("{messageId:int}")]
            [ProducesResponseType(typeof(MessageResponseDto), 200)]
            [ProducesResponseType(401)]
            [ProducesResponseType(404)]
            public async Task<IActionResult> GetMessageById(int messageId)
            {
                var currentUserId = CurrentUserId;
                if (currentUserId is null) return Unauthorized();

                var result = await _messageService.GetMessageByIdAsync(messageId, currentUserId);

                return result is null
                    ? NotFound(new { message = "Message not found." })
                    : Ok(result);
            }

            // ════════════════════════════════════════════════════════════════════════
            // 9. DELETE /api/messages/{messageId}
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Soft-deletes a message (sets IsDeleted = true in the database).
            /// Only the original SENDER may delete their own messages.
            /// The global query filter in MessageConfiguration ensures deleted messages
            /// never appear in any subsequent query.
            /// </summary>
            [HttpDelete("{messageId:int}")]
            [ProducesResponseType(204)]
            [ProducesResponseType(401)]
            [ProducesResponseType(403)]
            [ProducesResponseType(404)]
            public async Task<IActionResult> DeleteMessage(int messageId)
            {
                var currentUserId = CurrentUserId;
                if (currentUserId is null) return Unauthorized();

                var success = await _messageService.DeleteMessageAsync(messageId, currentUserId);

                return success
                    ? NoContent()
                    : NotFound(new { message = "Message not found or you are not the sender." });
            }

            // ════════════════════════════════════════════════════════════════════════
            // 10. GET /api/messages/online-status/{userId}
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>
            /// Checks whether a specific user currently has an active SignalR connection.
            /// Uses the static presence dictionary in ChatHub.
            /// </summary>
            [HttpGet("online-status/{userId}")]
            [ProducesResponseType(typeof(object), 200)]
            [ProducesResponseType(401)]
            public IActionResult GetOnlineStatus(string userId)
            {
                if (CurrentUserId is null) return Unauthorized();

                var isOnline = Presentation.Hubs.ChatHub.IsUserOnline(userId);
                return Ok(new { userId, isOnline });
            }
        }
    }

