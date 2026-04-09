using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Message
{
    //ده اللي إحنا هنبعته للموبايل عشان يعرضه في الشات
    public class MessageDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = default!;
        public string SenderName { get; set; } = default!; // مهمة عشان نعرض اسم اللي باعت
        public string ReceiverId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
