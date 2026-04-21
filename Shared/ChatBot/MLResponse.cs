using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ChatBot
{
    public class MLResponse
    {
        public string response { get; set; } // يجب أن يكون الاسم "response" حرفياً
        public int generation_time_ms { get; set; }
        public int tokens_generated { get; set; }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
