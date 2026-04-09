using DomainLayer.Contracts;
using DomainLayer.Models.Messages;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class MessageRepository : GenericRepository<Message,int>, IMessageRepository
    {
        private readonly StoreDbContext _context;

        public MessageRepository(StoreDbContext context) : base(context)
        {
            _context = context;
        }

        // هنجيب كل الرسايل اللي بين يوزرين عشان نعرض الشات بينهم
        public async Task<IEnumerable<Message>> GetConversationAsync(string userId, string otherUserId)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                            (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        // تعليم الرسايل كمقروءة لما يفتح الشات
        public async Task MarkAsReadAsync(string receiverId, string senderId)
        {
            var messages = await _context.Messages
                .Where(m => m.ReceiverId == receiverId && m.SenderId == senderId && !m.IsRead)
                .ToListAsync();

            messages.ForEach(m => m.IsRead = true);
            // التعديل بيتحفظ مع الـ UnitOfWork في الآخر
        }

        // هنجيب أحدث رسالة من كل شخص كلمناه (عشان صفحة الـ Inbox)
        public async Task<IEnumerable<Message>> GetUserConversationsAsync(string userId)
        {
            return await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => g.OrderByDescending(m => m.SentAt).FirstOrDefault()!) // ضيفنا علامة ! هنا
                .ToListAsync();
        }


    }
}
