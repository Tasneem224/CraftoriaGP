using Shared.ChatBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IChatBotService
    {
        Task<string> AskLlamaAsync(string userId, string message);
        Task<List<ChatBotMessagesDto>> GetChatHistoryAsync(string userId, int pageNumber, int pageSize);
        Task<string> GetWelcomeMessageAsync(string userId);
        Task<bool> UnloadModelAsync();

    }
}
