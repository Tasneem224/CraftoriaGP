using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Crmf;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.ChatBot;

[Authorize]
public class ChatBotController : BaseApiController
{
    private readonly IChatBotService _chatService;

    public ChatBotController(IChatBotService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("ask")]
    public async Task Ask([FromBody] ChatRequest requestData, CancellationToken cancellationToken)
    {
        // 1. استخراج التوكن (اللوجيك ده مكانه هنا لأنه متعلق بالـ Request)
        var token = Request.Headers["Authorization"].ToString();

        // 2. إعداد الـ Headers (نقلناها لميثود مساعدة تحت عشان الكود يبقى نظيف)
        PrepareSseResponse(Response);

        // 3. نداء الخدمة مباشرة
        await foreach (var chunk in _chatService.AskLlamaStreamingAsync(requestData.Message, token, cancellationToken))
        {
            await Response.WriteAsync($"data: {chunk}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }

    // ميثود مساعدة عشان متبوظش شكل الكود فوق
    private void PrepareSseResponse(HttpResponse response)
    {
        response.ContentType = "text/event-stream";
        response.Headers.Append("Cache-Control", "no-cache");
        response.Headers.Append("X-Accel-Buffering", "no");
        response.Headers.Append("Connection", "keep-alive");
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory() => SendSuccessResponse(await _chatService.GetChatHistoryAsync());

    [HttpGet("welcome")]
    public async Task<IActionResult> GetWelcome() => SendSuccessResponse(await _chatService.GetWelcomeMessageAsync());
}