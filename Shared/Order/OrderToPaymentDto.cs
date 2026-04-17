using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Order
{
    public class OrderToPaymentDto
    {
        public Guid OrderId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public List<OrderItemPaymentDto> Items { get; set; } = new();
    }
}
