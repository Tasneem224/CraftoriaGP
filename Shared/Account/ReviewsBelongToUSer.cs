using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Account
{
    public class ReviewsBelongToUSer
    {
        public string UserId { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string PicturUrl { get; set; } = string.Empty;
        public string? review { get; set; }  = string.Empty;
        public short? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
