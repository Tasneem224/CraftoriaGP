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
using static Google.Apis.Requests.BatchRequest;

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
        public async IAsyncEnumerable<string> AskLlamaStreamingAsync(string message, string token, [EnumeratorCancellation] CancellationToken ct)
        {
            // بنجهز الطلب زي ما الـ Swagger بتاعها طالب بالظبط
            var requestBody = new { message = message, max_tokens = 256, temperature = 0.4 };
            // التعديل: خلي المسار "chat/message" فقط بدون أي إضافات في الأول
            var request = new HttpRequestMessage(HttpMethod.Post, "chat/message")
            {
                Content = JsonContent.Create(new
                {
                    message = message,
                    max_tokens = 256,
                    temperature = 0.4
                })
            };

            if (!string.IsNullOrEmpty(token))
                request.Headers.Add("Authorization", token);

        
                var response = await _httpClient.SendAsync(request, ct);

                if (response.IsSuccessStatusCode)
                {
                    // بنقرأ الرد كنص خام الأول عشان لو فيه مشكلة تبان
                    var rawJson = await response.Content.ReadAsStringAsync(ct);

                    // فكي التشفير يدوي عشان تتأكدي إن الداتا موجودة
                    var result = JsonSerializer.Deserialize<MLResponse>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (result != null && !string.IsNullOrEmpty(result.response))
                    {
                        var words = result.response.Split(' ');
                        foreach (var word in words)
                        {
                            yield return word + " ";
                            await Task.Delay(40, ct);
                        }
                    }
                    else
                    {
                        yield return "DEBUG: الـ ML رد بـ JSON بس الـ response فاضي! الرد كان: " + rawJson;
                    }
                }
                else
                {
                    yield return $"خطأ من سيرفر الـ ML: {response.StatusCode}";
                
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
