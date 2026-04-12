using Shared.ChatBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IChatBotService
    {
        IAsyncEnumerable<string> AskLlamaStreamingAsync(string message, [EnumeratorCancellation] CancellationToken ct);
        Task<List<ChatBotMessagesDto>> GetChatHistoryAsync( int pageNumber, int pageSize);
        Task<string> GetWelcomeMessageAsync();

    }
}
