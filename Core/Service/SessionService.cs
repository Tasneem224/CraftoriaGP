using DomainLayer.Contracts;
using DomainLayer.Models;
using DomainLayer.Models.session;
using ServiceAbstraction;
using Shared.Session;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. إضافة خدمة (خبير)
        public async Task<object> AddExpertServiceAsync(string expertId, AddExpertServiceDto dto)
        {
            var service = new ExpertService
            {
                ExpertId = expertId,
                TitleAr = dto.TitleAr,
                TitleEn = dto.TitleEn,
                Price = dto.Price,
                DurationInMinutes = dto.DurationInMinutes
            };

            // استخدام الـ Generic Repository
            await _unitOfWork.GetRepository<ExpertService, int>().AddAsync(service);
            await _unitOfWork.SaveChanges();

            return new { Message = "تم إضافة الخدمة بنجاح", ServiceId = service.Id };
        }

        // 2. إضافة ميعاد متاح (خبير)
        public async Task<object> AddAvailabilityAsync(string expertId, AddAvailabilityDto dto)
        {
            var availability = new ExpertAvailability
            {
                ExpertId = expertId,
                Date = dto.Date.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsAvailable = true
            };

            await _unitOfWork.GetRepository<ExpertAvailability, int>().AddAsync(availability);
            await _unitOfWork.SaveChanges();

            return new { Message = "تم إضافة الموعد بنجاح", AvailabilityId = availability.Id };
        }

        // 3. جلب الخدمات (Localization)
        public async Task<object> GetExpertServicesAsync(string expertId)
        {
            bool isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";

            // استخدام GetAllQueryable عشان نقدر نفلتر قبل ما الداتا ترجع من الداتابيز
            var services = await _unitOfWork.GetRepository<ExpertService, int>()
                .GetAllQueryable()
                .Where(s => s.ExpertId == expertId)
                .Select(s => new ExpertServiceResponseDto
                {
                    Id = s.Id,
                    Title = isArabic ? s.TitleAr : s.TitleEn,
                    Price = s.Price,
                    DurationInMinutes = s.DurationInMinutes
                })
                .ToListAsync();

            return services;
        }

        // 4. جلب المواعيد المتاحة
        public async Task<object> GetExpertAvailabilitiesAsync(string expertId)
        {
            return await _unitOfWork.GetRepository<ExpertAvailability, int>()
                .GetAllQueryable()
                .Where(a => a.ExpertId == expertId && a.IsAvailable && a.Date >= DateTime.UtcNow.Date)
                .Select(a => new { a.Id, a.Date, a.StartTime, a.EndTime })
                .ToListAsync();
        }

        // 5. جلب حجوزات اليوزر (باستخدام الـ Specialized Repo لعمل الـ Include)
        public async Task<object> GetCustomerSessionsAsync(string customerId)
        {
            bool isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";

            // نستخدم الـ Repository الخاص اللي عملناه في الـ Unit of Work
            var sessions = await _unitOfWork.Sessions.GetCustomerSessionsWithDetailsAsync(customerId);

            return sessions.Select(s => new
            {
                SessionId = s.Id,
                ExpertId = s.ExpertId,
                // نستخدم الـ Navigation Property اللي الـ Repo عملها Include
                ServiceName = isArabic ? s.Service?.TitleAr : s.Service?.TitleEn,
                Date = s.Availability?.Date,
                StartTime = s.Availability?.StartTime,
                Status = GetLocalizedSessionStatus(s.Status, isArabic),
                AmountPaid = s.AmountPaid,
                // التعديل هنا: إضافة اللينك
                MeetingLink = string.IsNullOrEmpty(s.MeetingLink)
            ? (isArabic ? "في انتظار إضافة الرابط من الخبير" : "Waiting for link from expert")
            : s.MeetingLink
            }).ToList();
        }

        // 6. الحجز المبدئي (عملية مركبة)
        public async Task<object> BookSessionAsync(string beginnerId, BookSessionDto dto)
        {
            // 1. التأكد من الخدمة
            var service = await _unitOfWork.GetRepository<ExpertService, int>().GetByIdAsync(dto.ExpertServiceId);
            if (service == null || service.ExpertId != dto.ExpertId)
                throw new Exception("الخدمة غير موجودة.");

            // 2. التأكد من الموعد وقبضه (Update)
            var availabilityRepo = _unitOfWork.GetRepository<ExpertAvailability, int>();
            var availability = await availabilityRepo.GetByIdAsync(dto.ExpertAvailabilityId);

            if (availability == null || !availability.IsAvailable || availability.ExpertId != dto.ExpertId)
                throw new Exception("الموعد غير متاح.");

            // تحديث حالة الموعد
            availability.IsAvailable = false;
            availabilityRepo.Update(availability);

            // 3. إنشاء الجلسة
            var session = new Session
            {
                BeginnerId = beginnerId, // اليوزر الحالي هو المبتدئ
                ExpertId = dto.ExpertId, // الخبير اللي هو اختاره
                ExpertServiceId = service.Id,
                ExpertAvailabilityId = availability.Id,
                AmountPaid = service.Price,
                Status = SessionStatus.Pending,
                PaymentStatus = PaymentStatus.Pending
            };

            await _unitOfWork.Sessions.AddAsync(session);

            // 4. تنفيذ كل التغييرات في Transaction واحدة
            await _unitOfWork.SaveChanges();

            return new { SessionId = session.Id, AmountToPay = session.AmountPaid, Message = "تم الحجز المبدئي بنجاح." };
        }

        // 7. الدفع وتأكيد الحجز
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

        // Helper Method للترجمة
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

        // 1. جلب كل مواعيد الخبير (عشان يشوف الأجندة كاملة)
        // 1. جلب كل مواعيد الخبير (المتاحة وغير المتاحة)
        // 1. جلب كل مواعيد الخبير
        public async Task<IEnumerable<ExpertAvailabilityDto>> GetAllExpertAvailabilitiesAsync(string expertId)
        {
            // نستخدم الـ Generic Repository من الـ Unit of Work
            var availabilities = await _unitOfWork.GetRepository<ExpertAvailability, int>()
                .GetAllQueryable()
                .Where(a => a.ExpertId == expertId)
                .OrderBy(a => a.Date)
                .ToListAsync();

            // يفضل التحويل لـ DTO هنا
            return availabilities.Select(a => new ExpertAvailabilityDto
            {
                Id = a.Id,
                Date = a.Date,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                IsAvailable = a.IsAvailable
            });
        }

        // 2. تحديث رابط الاجتماع
        public async Task<bool> UpdateMeetingLinkAsync(int sessionId, string expertId, string meetingLink)
        {
            // نستخدم الـ Specialized Repo بتاع الـ Sessions اللي في الـ Unit of Work
            var session = await _unitOfWork.Sessions
                .GetAllQueryable()
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.ExpertId == expertId);

            if (session == null) return false;

            session.MeetingLink = meetingLink;

            _unitOfWork.Sessions.Update(session); // إبلاغ الـ Unit of Work بالتعديل
            await _unitOfWork.SaveChanges();      // تنفيذ الـ SaveChanges من الـ Unit of Work

            return true;
        }

        public async Task<object> GetExpertUpcomingSessionsAsync(string expertId)
        {
            bool isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";

            // هنجيب كل الجلسات (Sessions) اللي تخص الخبير ده وميعادها لسه مجاش
            var upcomingSessions = await _unitOfWork.Sessions.GetAllQueryable()
                .Include(s => s.Service)
                .Include(s => s.Availability)
                .Include(s => s.Beginner) // عشان نجيب اسم المبتدئ اللي حجز
                .Where(s => s.ExpertId == expertId && s.Availability.Date >= DateTime.UtcNow.Date)
                .OrderBy(s => s.Availability.Date)
                .ThenBy(s => s.Availability.StartTime)
                .Select(s => new
                {
                    SessionId = s.Id,
                    BeginnerName = s.Beginner.DisplayName ?? s.Beginner.FirstName, // اسم الشخص اللي حجز
                    ServiceName = isArabic ? s.Service.TitleAr : s.Service.TitleEn,
                    Date = s.Availability.Date,
                    StartTime = s.Availability.StartTime,
                    EndTime = s.Availability.EndTime,
                    Status = GetLocalizedSessionStatus(s.Status, isArabic),

                    // اللينك الحالي (لو فاضي هيظهر إنه محتاج يضيفه)
                    MeetingLink = s.MeetingLink ?? "لم يتم إضافة رابط بعد",

                    // عشان في الـ UI الموبايل يقدر يلون الحالة
                    NeedsAction = string.IsNullOrEmpty(s.MeetingLink)
                })
                .ToListAsync();

            return upcomingSessions;
        }
    }
}