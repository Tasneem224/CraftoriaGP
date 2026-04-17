using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ChatBot
{
    public class ChatStreamResponse
    {
        public List<ChatStreamChoice> Choices { get; set; }= new List<ChatStreamChoice>();
    }

    public class ChatStreamChoice
    {
        public ChatStreamDelta Delta { get; set; }= new ChatStreamDelta();
    }

    public class ChatStreamDelta
    {
        public string Content { get; set; }= string.Empty;
    }
}
