using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProductModule
{
    public class ReturnProductsOfCategory
    {
       public int Id { get; set; }
        public string Image { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; }= default!;
        public decimal Price { get; set; }

    }
}
