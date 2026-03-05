using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Order
{
    public class OrderToReturnDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        
        public string UserEmail { get; set; } = string.Empty;
        public string DeliveryMethod { get; set; } = string.Empty;
        public string OrderPaymentStatus { get; set; } = string.Empty;
        public decimal ShippingPrice { get; set; }
        public string Status { get; set; } = string.Empty;

        public IReadOnlyList<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; } 
    }
}
