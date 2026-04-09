using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [Authorize] // لازم اليوزر يكون عامل Login
    public class MessagesController : BaseApiController
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // إرسال رسالة جديدة
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(SendMessageDto dto)
        {
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId)) return Unauthorized();

            var result = await _messageService.SendMessageAsync(senderId, dto);
            return Ok(result);
        }

        // جلب المحادثة بين يوزر ويوزر تاني
        [HttpGet("chat/{otherUserId}")]
        public async Task<IActionResult> GetChat(string otherUserId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var chat = await _messageService.GetConversationAsync(currentUserId!, otherUserId);
            return Ok(chat);
        }

        // جلب قائمة المحادثات (Inbox)
        [HttpGet("inbox")]
        public async Task<IActionResult> GetInbox()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var inbox = await _messageService.GetInboxAsync(currentUserId!);
            return Ok(inbox);
        }
    }
}
