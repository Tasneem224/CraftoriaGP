using DomainLayer.Contracts;
using Org.BouncyCastle.Asn1.Cmp;
using ServiceAbstraction;
using Shared.ChatBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ChatBotService : IChatBotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;

        public ChatBotService(IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
        }
        public async Task<string> AskLlamaAsync(string userId, string message)
        {
            await _unitOfWork.ChatBot.AddMessages(message, userId, "user");
            await _unitOfWork.SaveChanges();

            var history = await _unitOfWork.ChatBot.GetLast5Messages(userId);
            var context = string.Join("\n", history.Select(m => $"{m.Role}: {m.Content}"));
            var fullMessage = $"{context}\nuser: {message}";

            var chatRequest = new
            {
                message = fullMessage,
                max_tokens = 256,
                temperature = 0.4
            };
            var response = await _httpClient.PostAsJsonAsync("chat/message", chatRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ChatResponseDto>();

                if (result != null)
                {
                    // 5. حفظ رد الـ AI في الداتابيز (Role: assistant)
                    await _unitOfWork.ChatBot.AddMessages(result.Response, userId, "assistant");
                    await _unitOfWork.SaveChanges();

                    return result.Response;
                }
            }
            else if ((int)response.StatusCode == 503)
            {
                return "الموديل بيحمل (Lazy Loading)، ثواني وجرب تاني.";
            }

            return "عذراً، حصلت مشكلة في التواصل مع البوت.";
        }

        public async Task<List<ChatBotMessagesDto>> GetChatHistoryAsync(string userId, int pageNumber, int pageSize)
        {
            var messages = await _unitOfWork.ChatBot.PaginationMessages(userId, pageNumber, pageSize);

            return messages.Select(m => new ChatBotMessagesDto
            {
                UserId = m.UserId,
                Role = m.Role,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            }).ToList();
        }

        public async Task<string> GetWelcomeMessageAsync(string userId)
        {
            // نتحقق لو اليوزر ده أول مرة يفتح الشات
            var messages = await _unitOfWork.ChatBot.PaginationMessages(userId, 1, 1);

            if (!messages.Any())
            {
                string welcome = "أهلاً بك! أنا لاما، مساعدك الذكي. كيف يمكنني مساعدتك اليوم؟";
                await _unitOfWork.ChatBot.AddMessages(welcome, userId, "assistant");
                await _unitOfWork.SaveChanges();
                return welcome;
            }
            return string.Empty;
        }

        public async Task<bool> UnloadModelAsync()
        {
            var response = await _httpClient.PostAsync("chat/unload", null);
            return response.IsSuccessStatusCode;
        }
    }
}
