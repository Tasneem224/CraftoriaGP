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
        Task<List<ChatBotMessages>> GetAllMessagesAsync(string userId);
        Task<ChatBotMessages> AddMessages(string message,string userId,string Role);
        Task<List< ChatBotMessages>> GetLast5Messages(string userId);
        Task<List<ChatBotMessages>> PaginationMessages(string userId,int pageNumber,int pageSize);
    }

}
