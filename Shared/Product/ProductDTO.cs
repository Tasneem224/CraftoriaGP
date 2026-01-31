using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }=default!;
        public string? ImageUrl { get; set; } = default!;
        public decimal Price { get; set; } = default!;

    }
}
