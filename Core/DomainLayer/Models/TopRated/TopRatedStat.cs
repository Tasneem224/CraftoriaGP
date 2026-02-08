using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.TopRated
{
    public class TopRatedStat
    {
        public int ProductId { get; set; }        // Product Id
        public string SellerId { get; set; }  // Seller/User Id
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
