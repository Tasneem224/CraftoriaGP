using Shared.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface ISessionService
    {
        Task<object> AddExpertServiceAsync(string expertId, AddExpertServiceDto dto);
        Task<object> AddAvailabilityAsync(string expertId, AddAvailabilityDto dto);

        Task<object> GetExpertServicesAsync(string expertId);
        Task<object> GetExpertAvailabilitiesAsync(string expertId);
        Task<object> GetCustomerSessionsAsync(string customerId); // إضافة جديدة عشان الـ UI

        Task<object> BookSessionAsync(string customerId, BookSessionDto dto);
        Task<object> MockPaymentAsync(int sessionId);
        // ميثود تجيب كل مواعيد الخبير (متاح وغير متاح)
        Task<IEnumerable<ExpertAvailabilityDto>> GetAllExpertAvailabilitiesAsync(string expertId);


        // ميثود تسمح للخبير بإضافة أو تحديث رابط الاجتماع لجلسة معينة
        Task<bool> UpdateMeetingLinkAsync(int sessionId, string expertId, string meetingLink);
        Task<object> GetExpertUpcomingSessionsAsync(string expertId);
        Task<ExpertDetailsResponse> GetExpertDetailsAsync(string expertId);
        Task<int>NumberOfSessionsForExpertAsync(string expertId);
        Task<IEnumerable<ExpertPastSessionDto>> GetExpertPastSessionsAsync(string expertId);
        Task<IEnumerable<CustomerPastSessionDto>> GetCustomerPastSessionsAsync(string customerId);
        Task<object> GetExpertSessionRequestsAsync(string expertId);
        Task<bool> CompleteSessionAsync(int sessionId, string beginnerId);
        

    }
}
