using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Http;
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
        public async Task Ask([FromBody] string message, CancellationToken cancellationToken)
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");
            Response.Headers.Append("X-Accel-Buffering", "no"); // مهم جداً للـ Hosting

            try
            {
                await Response.WriteAsync("data: Connecting...\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);

                await foreach (var chunk in _chatService.AskLlamaStreamingAsync(message, cancellationToken))
                {
                    await Response.WriteAsync($"data: {chunk}\n\n", cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken); // إرسال فوري
                }
            }
            catch (OperationCanceledException) { }
        }
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int size = 10)
        =>SendSuccessResponse(await _chatService.GetChatHistoryAsync(page, size));
          
        [HttpGet("welcome")]
        public async Task<IActionResult> GetWelcome()
        =>SendSuccessResponse(await _chatService.GetWelcomeMessageAsync());
        
    }
}
