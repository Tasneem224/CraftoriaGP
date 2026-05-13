using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CommunityModule
{
    public class PostResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } // المعرف الخاص بكاتب المنشور
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsLikedByMe { get; set; } // عشان الموبايل يلون القلب أحمر أو لأ
    }
}
