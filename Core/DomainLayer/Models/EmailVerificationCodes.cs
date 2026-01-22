using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class EmailVerificationCodes
    {
        public int Id {  get; set; }
        public string Email { get; set; } = default!;
        public string OTP { get;set; } = default!;
        public DateTime ExpirationTime { get; set; } = default!;
        public bool IsUsed { get; set; }
        public bool IsVerified { get; set; }
        public bool IsOld { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public int? AttemptCount { get; set; } = 5;
    }
}
