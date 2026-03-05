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
    }
}
