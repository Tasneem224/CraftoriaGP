using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Order
{
    public class Address
    {
        public string FullName { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string State { get; set; } = default!;
        public string City { get; set; } = default!;
    }

}
