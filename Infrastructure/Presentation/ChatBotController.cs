using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class ChatBotController:BaseApiController
    {
        private readonly IChatBotService _chatService;

        public ChatBotController(IChatBotService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] string message)
        =>
 
            SendSuccessResponse( await _chatService.AskLlamaAsync(message),"chat bot recive messages and reply to it successfuly");


  
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int size = 10)
        {

            var history = await _chatService.GetChatHistoryAsync(page, size);
            return Ok(history);
        }

        /// <summary>
        /// رسالة ترحيب لليوزر الجديد
        /// </summary>
        [HttpGet("welcome")]
        public async Task<IActionResult> GetWelcome()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var welcomeMsg = await _chatService.GetWelcomeMessageAsync(userId);
            return Ok(new { message = welcomeMsg });
        }

        /// <summary>
        /// تفريغ رامات الـ GPU (Memory Management)
        /// </summary>
        [HttpPost("unload")]
        public async Task<IActionResult> UnloadModel()
        {
            // ممكن تخليها للـ Admin بس لو حابة [Authorize(Roles = "Admin")]
            var success = await _chatService.UnloadModelAsync();

            if (success)
                return Ok("تم تفريغ ذاكرة الـ GPU بنجاح.");

            return StatusCode(500, "فشل تفريغ الذاكرة.");
        }
    }
}
