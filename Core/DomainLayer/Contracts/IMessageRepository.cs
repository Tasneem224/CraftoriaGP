using DomainLayer.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IMessageRepository : IGenericRepository<Message,int>
    {
        // نجيب المحادثة بين يوزرين (الرسايل اللي بينهم)
        Task<IEnumerable<Message>> GetConversationAsync(string userId, string otherUserId);

        // نجيب أحدث رسايل لكل المحادثات (لصفحة الـ Inbox)
        Task<IEnumerable<Message>> GetUserConversationsAsync(string userId);

        // تعليم الرسايل كمقروءة
        Task MarkAsReadAsync(string receiverId, string senderId);
    }
}
