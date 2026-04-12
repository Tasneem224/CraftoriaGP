using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Exceptions.DomainLayer.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Org.BouncyCastle.Asn1.Cmp;
using ServiceAbstraction;
using Shared.ChatBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class ChatBotService : IChatBotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatBotService(IUnitOfWork unitOfWork, HttpClient httpClient,IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }
        public async IAsyncEnumerable<string> AskLlamaStreamingAsync(string message, [EnumeratorCancellation] CancellationToken ct)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");
            var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            // 1. حفظ رسالة المستخدم
            await _unitOfWork.ChatBot.AddMessages(message, userId, "user");
            await _unitOfWork.SaveChanges();

            var chatRequest = new
            {
                message = message,
                max_tokens = 512,
                temperature = 0.4
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "chat/message")
            {
                Content = JsonContent.Create(chatRequest)
            };

            if (!string.IsNullOrEmpty(token))
                request.Headers.Add("Authorization", token);

            HttpResponseMessage? response = null;
            string? errorMessage = null;

            try
            {
                // محاولة إرسال الطلب
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

                if (!response.IsSuccessStatusCode)
                {
                    errorMessage = isArabic
                        ? "عذراً، الموديل غير متاح حالياً، برجاء المحاولة لاحقاً."
                        : "Sorry, the model is currently unavailable. Please try again later.";
                }
            }
            catch (Exception)
            {
                errorMessage = isArabic
                    ? "حدث خطأ في الاتصال بسيرفر البوت."
                    : "A connection error occurred with the chatbot server.";
            }

            // لو فيه خطأ، ابعتي الرسالة واخرجي من الميثود (خارج الـ try-catch)
            if (errorMessage != null)
            {
                yield return errorMessage;
                yield break;
            }

            // 3. قراءة الـ Stream (خارج الـ try-catch الرئيسي)
            using var stream = await response!.Content.ReadAsStreamAsync(ct);
            using var reader = new StreamReader(stream);
            string fullAiResponse = "";

            while (!reader.EndOfStream && !ct.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string? contentToEmit = null;

                if (line.StartsWith("data: "))
                {
                    var json = line.Substring(6);
                    if (json == "[DONE]") break;

                    try
                    {
                        var chunk = JsonSerializer.Deserialize<ChatStreamResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        contentToEmit = chunk?.Choices?[0]?.Delta?.Content;
                    }
                    catch { continue; }
                }

                if (!string.IsNullOrEmpty(contentToEmit))
                {
                    fullAiResponse += contentToEmit;
                    yield return contentToEmit;
                }
            }

            // 4. حفظ الرد النهائي
            if (!string.IsNullOrEmpty(fullAiResponse))
            {
                await _unitOfWork.ChatBot.AddMessages(fullAiResponse, userId, "assistant");
                await _unitOfWork.SaveChanges();
            }
        }
        public async Task<List<ChatBotMessagesDto>> GetChatHistoryAsync( int pageNumber, int pageSize)
        {
                        var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = await _unitOfWork.ChatBot.PaginationMessages(userId, pageNumber, pageSize);

            return messages.Select(m => new ChatBotMessagesDto
            {
                UserId = m.UserId,
                Role = m.Role,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            }).ToList();
        }

        public async Task<string> GetWelcomeMessageAsync()
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = await _unitOfWork.ChatBot.PaginationMessages(userId, 1, 1);

            if (!messages.Any())
            {
                string welcome = isArabic?"أهلاً بك! أنا لاما، مساعدك الذكي. كيف يمكنني مساعدتك اليوم؟":"";
                await _unitOfWork.ChatBot.AddMessages(welcome, userId, "assistant");
                await _unitOfWork.SaveChanges();
                return welcome;
            }
            return string.Empty;
        }

    }
}
