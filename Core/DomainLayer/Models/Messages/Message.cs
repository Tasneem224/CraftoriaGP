using DomainLayer.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Messages
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        // Navigation Properties (عشان نربطهم بجدول اليوزرز)
        // غيري ApplicationUser لاسم كلاس اليوزر عندك لو مختلف
        [ForeignKey(nameof(SenderId))]
        public virtual ApplicationUser Sender { get; set; } = default!;

        [ForeignKey(nameof(ReceiverId))]
        public virtual ApplicationUser Receiver { get; set; } = default!;
    }
}
