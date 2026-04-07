using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.ChatBot
{
    public class ChatBotSession
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }        
        public DateTime CreatedAt { get; set; }
        public List<ChatBotMessages> Messages { get; set; } = new();
    }

}
