using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Search
{
    public class ReturnSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Image { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public decimal Rating { get; set; } = default!;
    }
}
