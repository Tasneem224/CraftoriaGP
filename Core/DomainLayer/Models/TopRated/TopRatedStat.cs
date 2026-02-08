using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.TopRated
{
    public class TopRatedStat
    {
        // خليناها Nullable عشان لو بنجيب إحصائيات بائع أو خامة، ميكونش فيه رقم منتج (0)
        public int? ProductId { get; set; }

        public string? SellerId { get; set; }

        // 👇 دي الإضافة الجديدة المهمة
        public int? RawMaterialId { get; set; }

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
