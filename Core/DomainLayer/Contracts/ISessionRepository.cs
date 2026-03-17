using DomainLayer.Models.session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface ISessionRepository : IGenericRepository<Session, int>
    {
        // ميثود خاصة لجلب حجوزات العميل مع البيانات المرتبطة
        Task<IEnumerable<Session>> GetCustomerSessionsWithDetailsAsync(string customerId);
        Task<IEnumerable<Session>> GetExpertPastSessionsAsync(string expertId);
        Task<IEnumerable<Session>> GetCustomerPastSessionsAsync(string beginnerId);
    }
}
