using DomainLayer.Contracts;
using DomainLayer.Models.Messages;
using Microsoft.AspNetCore.SignalR;
using ServiceAbstraction;
//using ServiceAbstraction.Hubs; // تأكدي إن ده الـ namespace بتاع الـ IChatClient
using Shared.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        // تعديل: بنستخدم Hub العام عشان نهرب من مشكلة الـ Circular Dependency
        private readonly IHubContext<Hub> _hubContext;

        // تعديل: لازم تحقني الـ _hubContext هنا في الـ Constructor
        public MessageService(IUnitOfWork unitOfWork, IHubContext<Hub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<MessageDto> SendMessageAsync(string senderId, SendMessageDto messageDto)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = messageDto.ReceiverId,
                Content = messageDto.Content,
                SentAt = DateTime.Now
            };

            await _unitOfWork.Messages.AddAsync(message);
            await _unitOfWork.SaveChanges();

            // تعديل: لازم نجهز الـ DTO الأول عشان نبعته في الـ SignalR ونرجعه في الـ Return
            var resultDto = new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                SenderId = message.SenderId,
                SenderName = "Me" ,// أو هاتي اسم السندر لو محتاجاه
                ReceiverId = message.ReceiverId, // ضيفي السطر ده عشان ميبقاش Null
                SentAt = message.SentAt
            };

            // 2. إرسال الرسالة فوري للمستلم (الآن resultDto معروف)
            await _hubContext.Clients.User(messageDto.ReceiverId).SendAsync("ReceiveMessage", resultDto);

            return resultDto;
        }

        public async Task<IEnumerable<MessageDto>> GetConversationAsync(string userId, string otherUserId)
        {
            var messages = await _unitOfWork.Messages.GetConversationAsync(userId, otherUserId);

            return messages.Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                SentAt = m.SentAt,
                IsRead = m.IsRead
            });
        }

        public async Task<IEnumerable<MessageDto>> GetInboxAsync(string userId)
        {
            var lastMessages = await _unitOfWork.Messages.GetUserConversationsAsync(userId);
            return lastMessages.Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                SenderName = m.Sender?.UserName ?? "User",
                SentAt = m.SentAt,
                IsRead = m.IsRead
            });
        }
    }
}