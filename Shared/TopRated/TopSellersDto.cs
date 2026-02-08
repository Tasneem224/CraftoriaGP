using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.TopRated
{
    public class TopSellersDto
    {
        public string SellerId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Speciality { get; set; } // التخصص (Ceramics Expert)
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
