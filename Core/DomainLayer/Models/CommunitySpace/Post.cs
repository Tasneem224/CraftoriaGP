using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.CommunitySpace
{
    public class Post : BaseEntity<int>
    {
        public string Content { get; set; } // نص البوست
        public string? ImageUrl { get; set; } // رابط الصورة من Cloudinary
        public string? PublicId { get; set; } // بنحتاجه عشان لو هنمسح الصورة من Cloudinary
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // علاقة مع اليوزر (صاحب البوست)
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        

        // العلاقات التانية
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    }
}
