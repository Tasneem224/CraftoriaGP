using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Profile
{
    public class ReviewsProfile
    {
        public string? Review { get; set; } = default!;
        public short? Rating { get; set; } = default!;
        public string ItemImage { get; set; }= default!;
        public int ItemId { get; set; } = default!;
        public string? UserId { get; set; } = default!;
        public int ReviewId { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
    }
}
