using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProductModule
{
    public class UpdateProductDto 
    {
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public decimal? Price { get; set; } // لاحظي الـ ? هنا
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public int? Quantity { get; set; }
        public IFormFile? ImageFile { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string SellerName { get; set; } = default!;

    }
}
