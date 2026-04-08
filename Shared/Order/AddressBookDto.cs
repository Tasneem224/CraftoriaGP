using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Order
{
    public class AddressBookDto
    {
        public string FullName { get; set; } = default!;
        public string AppUserId { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string City { get; set; } = default!;
        public string StreetDetails { get; set; } = default!;
        public string Region { get; set; } = default!;
        
    }
}
