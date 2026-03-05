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
    // 1. جدول الخدمات (زي مراجعة بورتفوليو)
    public class ExpertService : BaseEntity<int>
    {
        public string ExpertId { get; set; }

        [ForeignKey(nameof(ExpertId))]
        public virtual ApplicationUser Expert { get; set; } // ربط بالخبير

        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
