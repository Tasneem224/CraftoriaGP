using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.TopRated
{
    public class TopProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }      // السعر ($45)
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
