using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Chat
{
    /// <summary>
    /// Optional query-string parameters for paginating a conversation.
    /// </summary>
    public class ConversationRequestDto
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 20;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
