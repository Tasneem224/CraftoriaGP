using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Identity
{
    public class VendorWallet: BaseEntity<int>
    {
        public string VendorId { get; set; } // الـ SellerId
        public decimal Balance { get; set; } // الرصيد الحالي
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
