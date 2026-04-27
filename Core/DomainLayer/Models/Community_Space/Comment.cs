using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Community_Space
{
    public class Comment:BaseEntity<int>

    {
        public string Text { get; set; }= string.Empty;
        public int AuthorId { get; set; }
        [ForeignKey(nameof(AuthorId))]
        public ApplicationUser Author { get; set; }
        public int PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
        public int? ParentCommentId { get; set; }
        [ForeignKey(nameof(ParentCommentId))]
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new HashSet<Comment>();
    }
}
