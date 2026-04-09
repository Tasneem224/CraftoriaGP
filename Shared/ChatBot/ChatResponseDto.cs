using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ChatBot
{
    public class ChatResponseDto
    {
        public string Response { get; set; } = string.Empty;
        public string InputMessage { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int TokensGenerated { get; set; }
        public double GenerationTimeMs { get; set; }
    }

}
