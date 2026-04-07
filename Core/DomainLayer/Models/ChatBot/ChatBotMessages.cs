using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.ChatBot
{
    public class ChatBotMessages
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Content { get; set; }
        public string role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
