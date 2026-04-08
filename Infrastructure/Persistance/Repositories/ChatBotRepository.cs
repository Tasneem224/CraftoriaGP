using DomainLayer.Contracts;
using DomainLayer.Models.ChatBot;
using Google;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MailKit.Net.Imap.ImapEvent;

namespace Persistance.Repositories
{
    public class ChatBotSessionRepository : IChatBotSessionRepository
    {
        private readonly StoreDbContext _context;

        public ChatBotSessionRepository(StoreDbContext context)
        {
            _context = context;
        }
        public async Task<ChatBotMessages> AddMessages(string message, string userId, string Role)
        {
            var newMessage = new ChatBotMessages
            {
                                Content=message,
                                UserId=userId,
                                Role= Role  
            };
            await _context.ChatBotMessages.AddAsync(newMessage);
            await _context.SaveChangesAsync();
            return newMessage;
        }

        public async Task<List<ChatBotMessages>> GetLast5Messages(string userId)
        {
            var messages = await _context.ChatBotMessages
          .Where(m => m.UserId == userId)
          .AsNoTracking()
          .OrderByDescending(m => m.CreatedAt) 
          .Take(5) 
          .ToListAsync();

            messages.Reverse();

            return messages ?? new List<ChatBotMessages>();
        }

        public async Task<List<ChatBotMessages>> PaginationMessages(string userId, int pageNumber, int pageSize)
        {
            var messages= await _context.ChatBotMessages
                .Where(m => m.UserId == userId)
                .AsNoTracking()
                .OrderByDescending(m=>m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return messages??new List<ChatBotMessages>() ;

        }
    }
}
