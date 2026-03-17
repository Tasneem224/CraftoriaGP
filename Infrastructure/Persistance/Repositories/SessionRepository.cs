using DomainLayer.Contracts;
using DomainLayer.Models.session;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class SessionRepository : GenericRepository<Session, int>, ISessionRepository
    {
        private readonly StoreDbContext _context;
        public SessionRepository(StoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Session>> GetCustomerSessionsWithDetailsAsync(string beginnerId)
        {
            return await _context.Sessions
                .Include(s => s.Service)      // عشان نجيب اسم الخدمة مترجم
                .Include(s => s.Availability) // عشان نجيب ميعاد الجلسة
                .Where(s => s.BeginnerId == beginnerId)
                .ToListAsync();
        }
        // ✅ جديد — جلسات الخبير اللي خلصت (Past)
        public async Task<IEnumerable<Session>> GetExpertPastSessionsAsync(string expertId)
        {
            return await _context.Sessions
                .Include(s => s.Service)
                .Include(s => s.Availability)
                .Include(s => s.Beginner)       // اسم المبتدئ اللي حجز
                .Where(s => s.ExpertId == expertId
                         && s.Availability.Date < DateTime.UtcNow.Date) // تاريخ قبل النهارده = Past
                .OrderByDescending(s => s.Availability.Date)            // الأحدث أولاً
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetCustomerPastSessionsAsync(string beginnerId)
        {
            return await _context.Sessions
                .Include(s => s.Service)
                .Include(s => s.Availability)
                .Include(s => s.Expert)        // عشان نجيب اسم الخبير وبياناته
                .Where(s => s.BeginnerId == beginnerId
                         && s.Availability.Date < DateTime.UtcNow.Date)
                .OrderByDescending(s => s.Availability.Date)
                .ToListAsync();
        }
    }
}
