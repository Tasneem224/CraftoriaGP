using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BasketModule
{
    public record CartDto
    {
        public string Id { get; init; } = default!;
        public ICollection<CartItemDto> basketItems { get; init; } = [];

    }
}
