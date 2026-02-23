using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.CartModule
{
    public class CustomerCart
    {
        public string Id { get; set; }
        public ICollection<CartItem> cartItems { get; set; } = [];
    }
}
