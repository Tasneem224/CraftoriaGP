using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Exceptions.DomainLayer.Exceptions;
using DomainLayer.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ChatBotService> _logger; // 1. تعريف المتغير
        public ChatBotService(UserManager<ApplicationUser> userManager,ILogger<ChatBotService> logger,IUnitOfWork unitOfWork, HttpClient httpClient,IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async IAsyncEnumerable<string> AskLlamaStreamingAsync(string message, string token, [EnumeratorCancellation] CancellationToken ct)
        {
            string errorMessage = null;
            string successfulResponse = null;
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exist = await _userManager.FindByIdAsync(userId);
            if (exist is null)
                throw new UserNotFoundException("no email found");
            // 1. العمليات التي قد تسبب Exceptions (بدون yield)
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "chat/message")
                {
                    Content = JsonContent.Create(new { message, max_tokens = 256, temperature = 0.4 })
                };

                if (!string.IsNullOrEmpty(token))
                    request.Headers.TryAddWithoutValidation("Authorization", token);

                var response = await _httpClient.SendAsync(request, ct);

                if (response.IsSuccessStatusCode)
                {
                    var rawJson = await response.Content.ReadAsStringAsync(ct);
                    var result = JsonSerializer.Deserialize<MLResponse>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    successfulResponse = result?.response;
                await _unitOfWork.ChatBot.AddMessages(message, userId, "client");
                await _unitOfWork.ChatBot.AddMessages(result.response, userId, "assistant");
                }
                else
                {
                    errorMessage = $"سيرفر الـ ML رد بخطأ: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الاتصال");
                errorMessage = "تعذر الاتصال بسيرفر الذكاء الاصطناعي.";
            }

            // 2. الآن نقوم بعمل الـ yield خارج بلوكات الـ try-catch
            if (errorMessage != null)
            {
                yield return errorMessage;
                yield break; // إنهاء الدالة
            }
                
            if (successfulResponse != null)
            {
                var words = successfulResponse.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    yield return word + " ";
                    await Task.Delay(40, ct);
                }
            }
        }                //{
                          //    // بنجهز الطلب زي ما الـ Swagger بتاعها طالب بالظبط
                          //    var requestBody = new { message = message, max_tokens = 256, temperature = 0.4 };
                          //    // التعديل: خلي المسار "chat/message" فقط بدون أي إضافات في الأول
                          //    var request = new HttpRequestMessage(HttpMethod.Post, "chat/message")
                          //    {
                          //        Content = JsonContent.Create(new
                          //        {
                          //            message = message,
                          //            max_tokens = 256,
                          //            temperature = 0.4
                          //        })
                          //    };

        //    if (!string.IsNullOrEmpty(token))
        //        request.Headers.Add("Authorization", token);


        //        var response = await _httpClient.SendAsync(request, ct);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // بنقرأ الرد كنص خام الأول عشان لو فيه مشكلة تبان
        //            var rawJson = await response.Content.ReadAsStringAsync(ct);

        //            // فكي التشفير يدوي عشان تتأكدي إن الداتا موجودة
        //            var result = JsonSerializer.Deserialize<MLResponse>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //            if (result != null && !string.IsNullOrEmpty(result.response))
        //            {
        //                var words = result.response.Split(' ');
        //                foreach (var word in words)
        //                {
        //                    yield return word + " ";
        //                    await Task.Delay(40, ct);
        //                }
        //            }
        //            else
        //            {
        //                yield return "DEBUG: الـ ML رد بـ JSON بس الـ response فاضي! الرد كان: " + rawJson;
        //            }
        //        }
        //        else
        //        {
        //            yield return $"خطأ من سيرفر الـ ML: {response.StatusCode}";

        //    }
        //}
        public async Task<List<ChatBotMessagesDto>> GetChatHistoryAsync()
        {
            try
            {


            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = await _unitOfWork.ChatBot.GetAllMessagesAsync(userId);

            return messages.Select(m => new ChatBotMessagesDto
            {
                UserId = m.UserId,
                Role = m.Role,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            }).ToList();
            }
            catch (Exception ex)
            {

          
                // هنا بنرمي الـ Exception الحقيقي عشان الـ Controller يمسكه
                var realMessage = ex.InnerException?.InnerException?.Message
                                  ?? ex.InnerException?.Message
                                  ?? ex.Message;

                throw new Exception("Database Error: " + realMessage);
            }
        }
        public async Task<string> GetWelcomeMessageAsync()
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exist =await _userManager.FindByIdAsync(userId);
            if (exist is null)
                throw new UserNotFoundException("no email found");
            var messages = await _unitOfWork.ChatBot.PaginationMessages(userId, 1, 1);

            if (!messages.Any())
            {
                string welcome = isArabic?"أهلاً بك! أنا لاما، مساعدك الذكي. كيف يمكنني مساعدتك اليوم؟" : "Welcome! I'm Lama, your AI assistant. How can I help you today?";
                await _unitOfWork.ChatBot.AddMessages(welcome, userId, "assistant");
                await _unitOfWork.SaveChangesAsync();
                return welcome;
            }
            return string.Empty;
        }

    }
}
