using Shared.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IMessageService
    {
        Task<MessageDto> SendMessageAsync(string senderId, SendMessageDto messageDto);
        Task<IEnumerable<MessageDto>> GetConversationAsync(string userId, string otherUserId);
        Task<IEnumerable<MessageDto>> GetInboxAsync(string userId);
    }
}
