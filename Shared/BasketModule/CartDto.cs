using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BasketModule
{
    public class CartDto
    {
        public string Id { get; set; } = default!;
        public ICollection<CartItemDto> cartItems { get; set; } = [];

    }
}
