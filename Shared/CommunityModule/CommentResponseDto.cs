using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CommunityModule
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; } // عشان يظهر "John Smith"
        public string? UserImageUrl { get; set; } // لو حابة تحطي صورة البروفايل
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; } // دي حركة "صياعة" عشان نبعت "1h ago" جاهزة للموبايل
    }
}
