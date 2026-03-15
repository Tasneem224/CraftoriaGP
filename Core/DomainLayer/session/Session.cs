using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using Shared.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.session
{
    // 3. جدول الحجز الفعلي
    public class Session : BaseEntity<int>
    {
        // البائع المبتدئ اللي طالب الجلسة
        public string BeginnerId { get; set; }
        [ForeignKey(nameof(BeginnerId))]
        public virtual ApplicationUser Beginner { get; set; }

        // الخبير اللي هيقدم الجلسة
        public string ExpertId { get; set; }
        [ForeignKey(nameof(ExpertId))]
        public virtual ApplicationUser Expert { get; set; }

        public int ExpertServiceId { get; set; }
        [ForeignKey(nameof(ExpertServiceId))]
        public virtual ExpertService Service { get; set; }

        public int ExpertAvailabilityId { get; set; }
        [ForeignKey(nameof(ExpertAvailabilityId))]
        public virtual ExpertAvailability Availability { get; set; }

        public decimal AmountPaid { get; set; }
        public SessionStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? MeetingLink { get; set; } // الرابط ممكن يكون نل لحد ما الخبير يحطه
        public TimeSpan EndTime { get; set; }
    }
}
