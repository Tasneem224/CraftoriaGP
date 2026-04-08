using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models;
using DomainLayer.Models.Identity;
using DomainLayer.Models.session;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction;
using Shared.Session;
using System.Globalization;

namespace Service
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public SessionService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // 1. إضافة خدمة (خبير)
        public async Task<object> AddExpertServiceAsync(string expertId, AddExpertServiceDto dto)
        {
            var service = new ExpertService
            {
                ExpertId = expertId,
                TitleAr = dto.TitleAr,
                TitleEn = dto.TitleEn,
                DescriptionAr = dto.DescriptionAr, // ✅ جديد
                DescriptionEn = dto.DescriptionEn, // ✅ جديد
                Price = dto.Price,
                DurationInMinutes = dto.DurationInMinutes
            };

            await _unitOfWork.GetRepository<ExpertService, int>().AddAsync(service);
            await _unitOfWork.SaveChanges();

            return new { Message = "تم إضافة الخدمة بنجاح", ServiceId = service.Id };
        }

        // 2. إضافة ميعاد متاح (خبير) — بدون EndTime، بتتحسب وقت الحجز
        public async Task<object> AddAvailabilityAsync(string expertId, AddAvailabilityDto dto)
        {
            var availability = new ExpertAvailability
            {
                ExpertId = expertId,
                Date = dto.Date.Date,
                StartTime = dto.StartTime,
                // ✅ مفيش EndTime هنا خالص
                IsAvailable = true
            };

            await _unitOfWork.GetRepository<ExpertAvailability, int>().AddAsync(availability);
            await _unitOfWork.SaveChanges();

            return new { Message = "تم إضافة الموعد بنجاح", AvailabilityId = availability.Id };
        }

        // 3. جلب الخدمات
        public async Task<object> GetExpertServicesAsync(string expertId)
        {
            bool isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";

            var services = await _unitOfWork.GetRepository<ExpertService, int>()
                .GetAllQueryable()
                .Where(s => s.ExpertId == expertId)
                .Select(s => new ExpertServiceResponseDto
                {
                    Id = s.Id,
                    Title = isArabic ? s.TitleAr : s.TitleEn,
                    Description = isArabic ? s.DescriptionAr : s.DescriptionEn, // ✅

                    Price = s.Price,
                    DurationInMinutes = s.DurationInMinutes
                })
                .ToListAsync();

            return services;
        }

        // 4. جلب المواعيد المتاحة — بدون EndTime
        public async Task<object> GetExpertAvailabilitiesAsync(string expertId)
        {
            return await _unitOfWork.GetRepository<ExpertAvailability, int>()
                .GetAllQueryable()
                .Where(a => a.ExpertId == expertId
                         && a.IsAvailable
                         && a.Date >= DateTime.UtcNow.Date)
                .Select(a => new { a.Id, a.Date, a.StartTime })
                .ToListAsync();
        }

        // 5. جلب جلسات البيجينر Upcoming مع JoinStatus
        public async Task<object> GetCustomerSessionsAsync(string customerId)
        {
            // استخدم التوقيت المحلي للسيرفر (Local) عشان يطابق المواعيد اللي الخبراء دخلوها غالباً
            var now = DateTime.Now;
            var today = now.Date; // هيرجع التاريخ فقط بدون وقت (00:00:00)
            var currentTime = now.TimeOfDay; // هيرجع الوقت الحالي (ساعات ودقائق وثواني)
            bool isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";

            var sessions = await _unitOfWork.Sessions
                .GetAllQueryable()
                .Include(s => s.Service)
                .Include(s => s.Availability)
                .Include(s => s.Expert)
                .Where(s => s.BeginnerId == customerId  && s.Status == SessionStatus.Confirmed) // Upcoming = Confirmed بس
                  // الفلتر الذكي: التاريخ لسه مجاش، أو التاريخ هو النهاردة بس وقت النهاية لسه مخلصش
                .Where(s => s.Availability.Date > today ||
                   (s.Availability.Date == today && s.EndTime > currentTime))
                .ToListAsync();

            return sessions.Select(s => new
            {
                SessionId = s.Id,
                ExpertId = s.ExpertId,
                ExpertName = s.Expert?.DisplayName ?? s.Expert?.FirstName,
                ServiceName = isArabic ? s.Service?.TitleAr : s.Service?.TitleEn,
                Date = s.Availability?.Date,
                StartTime = s.Availability?.StartTime,
                EndTime = s.EndTime,   // ✅ من الـ Session مش الـ Availability
                Status = GetLocalizedSessionStatus(s.Status, isArabic),
                AmountPaid = s.AmountPaid,
                MeetingLink = s.MeetingLink,

                // ✅ Flutter بتستخدمه تقرر تعمل إيه مع زرار Join
                JoinStatus = GetJoinStatus(s)
            }).ToList();
        }

        // 6. الحجز — بيحسب EndTime من الـ Service اللي اختارها البيجينر
        public async Task<object> BookSessionAsync(string beginnerId, BookSessionDto dto)
        {
            var service = await _unitOfWork.GetRepository<ExpertService, int>()
                            .GetByIdAsync(dto.ExpertServiceId);
            if (service == null || service.ExpertId != dto.ExpertId)
                throw new Exception("الخدمة غير موجودة.");

            var availabilityRepo = _unitOfWork.GetRepository<ExpertAvailability, int>();
            var availability = await availabilityRepo.GetByIdAsync(dto.ExpertAvailabilityId);

            if (availability == null || !availability.IsAvailable || availability.ExpertId != dto.ExpertId)
                throw new Exception("الموعد غير متاح.");

            availability.IsAvailable = false;
            availabilityRepo.Update(availability);

            // ✅ EndTime = StartTime + DurationInMinutes بتاع الخدمة اللي اختارها
            var endTime = availability.StartTime
                            .Add(TimeSpan.FromMinutes(service.DurationInMinutes));

            var session = new Session
            {
                BeginnerId = beginnerId,
                ExpertId = dto.ExpertId,
                ExpertServiceId = service.Id,
                ExpertAvailabilityId = availability.Id,
                AmountPaid = service.Price,
                Status = SessionStatus.Confirmed,
                PaymentStatus = PaymentStatus.Pending,
                EndTime = endTime  // ✅
            };

            await _unitOfWork.Sessions.AddAsync(session);
            await _unitOfWork.SaveChanges();

            return new
            {
                SessionId = session.Id,
                StartTime = availability.StartTime,
                EndTime = session.EndTime,
                AmountToPay = session.AmountPaid,
                Message = "تم الحجز بنجاح."
            };
        }

        // 7. إنهاء الجلسة وتحويلها لـ Completed
        public async Task<object> CompleteSessionAsync(int sessionId, string userId)
        {
            var session = await _unitOfWork.Sessions
                .GetAllQueryable()
                .Include(s => s.Availability)
                .FirstOrDefaultAsync(s => s.Id == sessionId
                                       && (s.BeginnerId == userId  // ✅ البيجينر
                                        || s.ExpertId == userId)); // ✅ أو الخبير

            if (session == null)
                return new { Success = false, Message = "الجلسة غير موجودة" };

            var sessionEnd = session.Availability.Date.Date + session.EndTime;
            if (DateTime.UtcNow < sessionEnd)
                return new { Success = false, Message = "لم تنته الجلسة بعد" };

            session.Status = SessionStatus.Completed;
            _unitOfWork.Sessions.Update(session);
            await _unitOfWork.SaveChanges();

            return new { Success = true, Message = "تم إنهاء الجلسة بنجاح" };
        }

        // 8. الدفع الوهمي — هيتشال لما تربطي Gateway
        public async Task<object> MockPaymentAsync(int sessionId)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(sessionId);
            if (session == null) throw new Exception("الجلسة غير موجودة");

            if (session.PaymentStatus == PaymentStatus.Paid)
                throw new Exception("تم الدفع مسبقاً");

            session.PaymentStatus = PaymentStatus.Paid;
            session.Status = SessionStatus.Confirmed;

            _unitOfWork.Sessions.Update(session);
            await _unitOfWork.SaveChanges();

            return new { SessionId = session.Id, Status = "Confirmed", Message = "تم الدفع وتأكيد الحجز." };
        }

        // 9. كل مواعيد الخبير
        public async Task<IEnumerable<ExpertAvailabilityDto>> GetAllExpertAvailabilitiesAsync(string expertId)
        {
            var availabilities = await _unitOfWork.GetRepository<ExpertAvailability, int>()
                .GetAllQueryable()
                .Where(a => a.ExpertId == expertId)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return availabilities.Select(a => new ExpertAvailabilityDto
            {
                Id = a.Id,
                Date = a.Date,
                StartTime = a.StartTime,
                // ✅ مفيش EndTime في الـ DTO بعد كده
                IsAvailable = a.IsAvailable
            });
        }

        // 10. تحديث رابط الاجتماع
        public async Task<bool> UpdateMeetingLinkAsync(int sessionId, string expertId, string meetingLink)
        {
            var session = await _unitOfWork.Sessions
                .GetAllQueryable()
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.ExpertId == expertId);

            if (session == null) return false;

            session.MeetingLink = meetingLink;
            _unitOfWork.Sessions.Update(session);
            await _unitOfWork.SaveChanges();

            return true;
        }

        // 11. Upcoming Sessions للخبير
        public async Task<object> GetExpertUpcomingSessionsAsync(string expertId)
        {
            bool isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";

            var now = DateTime.Now;
            var today = now.Date;
            var currentTime = now.TimeOfDay;

            var upcomingSessions = await _unitOfWork.Sessions.GetAllQueryable()
                .Include(s => s.Service)
                .Include(s => s.Availability)
                .Include(s => s.Beginner)
                .Where(s => s.ExpertId == expertId && s.Status == SessionStatus.Confirmed)
                .Where(s => s.Availability.Date > today ||
                           (s.Availability.Date == today && s.EndTime > currentTime))
                .OrderBy(s => s.Availability.Date)
                .ThenBy(s => s.Availability.StartTime)
                .Select(s => new
                {
                    SessionId = s.Id,
                    BeginnerName = s.Beginner.DisplayName ?? s.Beginner.FirstName,
                    ServiceName = isArabic ? s.Service.TitleAr : s.Service.TitleEn,
                    Date = s.Availability.Date,
                    StartTime = s.Availability.StartTime,
                    EndTime = s.EndTime,
                    Status = GetLocalizedSessionStatus(s.Status, isArabic),
                    MeetingLink = s.MeetingLink ?? "لم يتم إضافة رابط بعد",
                    NeedsAction = string.IsNullOrEmpty(s.MeetingLink)
                })
                .ToListAsync();

            return upcomingSessions;
        }

        // 12. Expert Details
        public async Task<ExpertDetailsResponse> GetExpertDetailsAsync(string expertId)
        {
            var user = await _userManager.FindByIdAsync(expertId);
            if (user == null)
                throw new UserNotFoundException("this user is not found");

            return new ExpertDetailsResponse
            {
                Name = user.DisplayName ?? user.FirstName,
                Description = user.Bio ?? "NO Description Available!",
                ImageUrl = user.ProfileImage ?? "No Image for this user"
            };
        }

        // 13. عدد الجلسات للخبير في الـ Dashboard
        public async Task<int> NumberOfSessionsForExpertAsync(string expertId)
        {
            return await _unitOfWork.Sessions.GetAllQueryable()
                .Where(s => s.ExpertId == expertId
                         && (s.Status == SessionStatus.Confirmed
                          || s.Status == SessionStatus.Completed)) // ✅ الكل مش بس Upcoming
                .CountAsync();
        }

        // 14. Past Sessions للخبير — بتاعت Completed بس
        public async Task<IEnumerable<ExpertPastSessionDto>> GetExpertPastSessionsAsync(string expertId)
        {
            bool isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";

            var now = DateTime.Now;
            var today = now.Date;
            var currentTime = now.TimeOfDay;

            var pastSessions = await _unitOfWork.Sessions.GetAllQueryable()
                .Include(s => s.Service)
                .Include(s => s.Availability)
                .Include(s => s.Beginner)
                .Where(s => s.ExpertId == expertId)
                .Where(s => s.Status == SessionStatus.Completed ||
                          (s.Status == SessionStatus.Confirmed &&
                           (s.Availability.Date < today || (s.Availability.Date == today && s.EndTime <= currentTime))))
                .OrderByDescending(s => s.Availability.Date)
                .ToListAsync();

            return pastSessions.Select(s => new ExpertPastSessionDto
            {
                SessionId = s.Id,
                BeginnerName = s.Beginner?.DisplayName ?? s.Beginner?.FirstName ?? "Unknown",
                ServiceName = isArabic ? s.Service?.TitleAr : s.Service?.TitleEn,
                Date = s.Availability.Date,
                Duration = $"{s.Availability.StartTime:hh\\:mm tt} • {s.Service?.DurationInMinutes} min",
                Status = GetLocalizedSessionStatus(s.Status, isArabic),
                AmountPaid = s.AmountPaid
            });
        }

        // 15. Past Sessions للبيجينر — Completed بس
        public async Task<IEnumerable<CustomerPastSessionDto>> GetCustomerPastSessionsAsync(string customerId)
        {
            bool isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";

            var now = DateTime.Now;
            var today = now.Date;
            var currentTime = now.TimeOfDay;

            var pastSessions = await _unitOfWork.Sessions.GetAllQueryable()
                .Include(s => s.Service)
                .Include(s => s.Availability)
                .Include(s => s.Expert)
                .Where(s => s.BeginnerId == customerId)
                // الفلتر: الحالة Completed صراحة OR الحالة Confirmed بس الوقت عدى خلاص
                .Where(s => s.Status == SessionStatus.Completed ||
                          (s.Status == SessionStatus.Confirmed &&
                           (s.Availability.Date < today || (s.Availability.Date == today && s.EndTime <= currentTime))))
                .OrderByDescending(s => s.Availability.Date)
                .ToListAsync();

            return pastSessions.Select(s => new CustomerPastSessionDto
            {
                SessionId = s.Id,
                ExpertId = s.ExpertId,
                ExpertName = s.Expert?.DisplayName ?? s.Expert?.FirstName ?? "Unknown",
                ExpertImageUrl = s.Expert?.ProfileImage ?? string.Empty,
                ServiceName = isArabic ? s.Service?.TitleAr : s.Service?.TitleEn,
                Date = s.Availability.Date,
                TimeAndDuration = $"{s.Availability.StartTime:hh\\:mm tt} • {s.Service?.DurationInMinutes} min",
                Status = GetLocalizedSessionStatus(s.Status, isArabic),
                AmountPaid = s.AmountPaid
            });
        }
        // 16. Requests للخبير — الجلسات المؤكدة اللي لسه محتاجة لينك
        public async Task<object> GetExpertSessionRequestsAsync(string expertId)
        {
            bool isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";

            return await _unitOfWork.Sessions.GetAllQueryable()
                .Include(s => s.Service)
                .Include(s => s.Availability)
                .Include(s => s.Beginner)
                .Where(s => s.ExpertId == expertId
                         && s.Status == SessionStatus.Confirmed
                         && string.IsNullOrEmpty(s.MeetingLink))
                .OrderBy(s => s.Availability.Date)
                .Select(s => new
                {
                    SessionId = s.Id,
                    BeginnerName = s.Beginner.DisplayName ?? s.Beginner.FirstName,
                    BeginnerImage = s.Beginner.ProfileImage,
                    ServiceName = isArabic ? s.Service.TitleAr : s.Service.TitleEn,
                    Date = s.Availability.Date,
                    StartTime = s.Availability.StartTime,
                    EndTime = s.EndTime  // ✅ من الـ Session
                })
                .ToListAsync();
        }

        // ✅ Helper — بيحدد حالة زرار Join للـ Flutter
        private static string GetJoinStatus(Session s)
        {
            // حالة 1 — مفيش لينك
            if (string.IsNullOrEmpty(s.MeetingLink))
                return "NO_LINK";

            var now = DateTime.UtcNow;
            var sessionStart = s.Availability.Date.Date + s.Availability.StartTime;
            var sessionEnd = s.Availability.Date.Date + s.EndTime;

            // حالة 2 — في لينك بس لسه مجاش الوقت
            if (now < sessionStart) return "NOT_YET";

            // حالة 3 — الوقت جه، يقدر يدخل
            if (now <= sessionEnd) return "JOIN_NOW";

            // الجلسة خلصت — Flutter هتكلم CompleteSession endpoint
            return "ENDED";
        }

        // ✅ Helper — ترجمة الـ Status
        private static string GetLocalizedSessionStatus(SessionStatus status, bool isArabic)
        {
            return status switch
            {
                SessionStatus.Pending => isArabic ? "قيد الانتظار" : "Pending",
                SessionStatus.Confirmed => isArabic ? "مؤكد" : "Confirmed",
                SessionStatus.Completed => isArabic ? "مكتمل" : "Completed",
                SessionStatus.Cancelled => isArabic ? "ملغي" : "Cancelled",
                _ => status.ToString()
            };
        }
    }
}