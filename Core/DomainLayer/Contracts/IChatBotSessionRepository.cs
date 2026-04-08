using DomainLayer.Models.ChatBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IChatBotSessionRepository
    {
        Task<ChatBotSession> CreateAsync(string userId);
        Task<ChatBotSession?> GetByIdAsync(Guid sessionId);
        Task SaveMessageAsync(ChatBotMessages message);
        Task<List<ChatBotMessages>> GetHistoryAsync(Guid sessionId);


    }

}
