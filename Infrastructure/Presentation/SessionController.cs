using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class SessionController : BaseApiController // توحيد شكل الردود (Success/Error)
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        #region Expert Endpoints (أدوات الخبير)

        // 1. الخبير يضيف خدمة (مثلاً: مراجعة بورتفوليو)
        [HttpPost("expert/add-service")]
        [Authorize(Roles = "Expert")] // مسموح فقط لمن لديه رول Expert
        public async Task<IActionResult> AddExpertService([FromBody] AddExpertServiceDto dto)
        {
            if (!ModelState.IsValid) return SendErrorResponse("بيانات غير صالحة", ModelState, 422);

            var expertId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _sessionService.AddExpertServiceAsync(expertId, dto);

            return SendSuccessResponse(result, "تم إضافة الخدمة بنجاح");
        }

        // 2. الخبير يضيف مواعيد متاحة في الأجندة بتاعته
        [HttpPost("expert/add-availability")]
        [Authorize(Roles = "Expert")]
        public async Task<IActionResult> AddAvailability([FromBody] AddAvailabilityDto dto)
        {
            if (!ModelState.IsValid) return SendErrorResponse("بيانات غير صالحة", ModelState, 422);

            var expertId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _sessionService.AddAvailabilityAsync(expertId, dto);

            return SendSuccessResponse(result, "تم إضافة الموعد بنجاح");
        }

        #endregion

        #region Public/Beginner View (العرض للمبتدئ)

        // 3. عرض خدمات خبير معين (مترجمة حسب لغة الموبايل)
        [HttpGet("expert/{expertId}/services")]
        public async Task<IActionResult> GetExpertServices(string expertId)
        {
            var result = await _sessionService.GetExpertServicesAsync(expertId);
            return SendSuccessResponse(result);
        }

        // 4. عرض المواعيد المتاحة عند خبير معين
        [HttpGet("expert/{expertId}/slots")]
        public async Task<IActionResult> GetExpertSlots(string expertId)
        {
            var result = await _sessionService.GetExpertAvailabilitiesAsync(expertId);
            return SendSuccessResponse(result);
        }

        #endregion

        #region Beginner Actions (أفعال المبتدئ)

        // 5. المبتدئ يحجز جلسة مع خبير
        [HttpPost("book")]
        [Authorize] // أي يوزر مسجل يقدر يحجز (باعتباره Beginner)
        public async Task<IActionResult> BookSession([FromBody] BookSessionDto dto)
        {
            if (!ModelState.IsValid) return SendErrorResponse("البيانات ناقصة", ModelState, 422);

            // اليوزر الحالي هو الـ Beginner
            var beginnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _sessionService.BookSessionAsync(beginnerId, dto);

            return SendSuccessResponse(result, "تم الحجز المبدئي، يرجى إتمام الدفع");
        }

        // 6. المبتدئ يشوف تاريخ جلساته (مترجمة)
        [HttpGet("beginner/my-sessions")]
        [Authorize]
        public async Task<IActionResult> GetMySessions()
        {
            var beginnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _sessionService.GetCustomerSessionsAsync(beginnerId);

            return SendSuccessResponse(result);
        }

        // 7. إتمام الدفع (وهمي) لتأكيد الجلسة
        [HttpPost("mock-pay/{sessionId}")]
        [Authorize]
        public async Task<IActionResult> MockPay(int sessionId)
        {
            var result = await _sessionService.MockPaymentAsync(sessionId);
            return SendSuccessResponse(result, "تم تأكيد الحجز بنجاح");
        }

        #endregion

        // الحصول على الأجندة كاملة
        [HttpGet("expert/my-all-slots")]
        [Authorize(Roles = "Expert")]
        public async Task<IActionResult> GetAllMySlots()
        {
            var expertId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _sessionService.GetAllExpertAvailabilitiesAsync(expertId);
            return SendSuccessResponse(result);
        }

        [HttpPut("expert/update-meeting-link/{sessionId}")]
        [Authorize(Roles = "Expert")]
        public async Task<IActionResult> UpdateLink(int sessionId, [FromBody] string meetingLink)
        {
            var expertId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _sessionService.UpdateMeetingLinkAsync(sessionId, expertId, meetingLink);

            if (!result) return SendErrorResponse("الجلسة غير موجودة أو غير مصرح لك بتعديلها", null, 404);

            // حددنا <object> هنا عشان الـ compiler يفهم الـ null
            return SendSuccessResponse<object>(null, "تم تحديث رابط الاجتماع بنجاح");
        }

        // 9. الخبير يشوف الجلسات اللي اتحجزت معاه (Upcoming Sessions)
        [HttpGet("expert/upcoming-sessions")]
        [Authorize(Roles = "Expert")]
        public async Task<IActionResult> GetUpcomingSessions()
        {
            var expertId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _sessionService.GetExpertUpcomingSessionsAsync(expertId);
            return SendSuccessResponse(result);
        }

        [HttpGet("expert/details")]
        public async Task<IActionResult> GetExpertDetails(string expertId)
        {
            //var expertId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            return SendSuccessResponse(await _sessionService.GetExpertDetailsAsync(expertId));
        }

        [HttpGet("expert/{expertId}/sessions/past")]
        public async Task<IActionResult> GetExpertPastSessions(string expertId)
        {
            var result = await _sessionService.GetExpertPastSessionsAsync(expertId);
            return SendSuccessResponse(result);
        }

        [HttpGet("customer/{customerId}/sessions/past")]
        public async Task<IActionResult> GetCustomerPastSessions(string customerId)
        {
            var result = await _sessionService.GetCustomerPastSessionsAsync(customerId);
            return SendSuccessResponse(result);
        }

        [HttpGet("expert/{expertId}/sessions/requests")]
        public async Task<IActionResult> GetExpertSessionRequests(string expertId)
        {
            var result = await _sessionService.GetExpertSessionRequestsAsync(expertId);
            return SendSuccessResponse(result);
        }
    }
}
