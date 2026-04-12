using DomainLayer.Exceptions;
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
       =>SendSuccessResponse(await _chatService.AskLlamaAsync(message));

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int size = 10)
        =>SendSuccessResponse(await _chatService.GetChatHistoryAsync(page, size));
          
        [HttpGet("welcome")]
        public async Task<IActionResult> GetWelcome()
        =>SendSuccessResponse(await _chatService.GetWelcomeMessageAsync());
        
    }
}
