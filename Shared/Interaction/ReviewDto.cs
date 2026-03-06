using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interaction
{
    public class ReviewDto
    {
        public int InteractionId { get; set; }
        public string? UserImage { get; set; }
        public string ReviewerId { get; set; }
        public string ReviewerName { get; set; }
        public short Rating { get; set; }
        public string? ReviewComment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
