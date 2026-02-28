using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.session
{
    // 2. جدول المواعيد المتاحة
    public class ExpertAvailability : BaseEntity<int>
    {
        public string ExpertId { get; set; }

        [ForeignKey(nameof(ExpertId))]
        public virtual ApplicationUser Expert { get; set; } // ربط بالخبير

        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
