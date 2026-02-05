using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interaction
{
    public class ReviewStatsDto
    {
        public int TotalReviews { get; set; } // عدد الريفيوهات (شخصي + منتجات)
        public double AverageRating { get; set; } // متوسط التقييم
    }
}
