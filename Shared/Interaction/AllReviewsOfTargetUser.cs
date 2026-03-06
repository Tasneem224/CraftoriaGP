using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interaction
{
    public class AllReviewsOfTargetUser
    {
        public int InteractionId { get; set; }
        public string ReviewerId { get; set; }
        public string? ReviewerImage { get; set; }
        public string ReviewerName { get; set; }
        public int  ItemId { get; set; }

        public short Rating { get; set; }
        public string? ReviewComment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
