using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProductModule
{
    
        public class ReturnProductDto
        {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // 👈 ده اللي الـ Flutter محتاجه

        public string SellerId { get; set; }
        public string SellerName { get; set; }

    }
}

