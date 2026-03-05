using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShippingAddress=DomainLayer.Models.Order.Address;
namespace DomainLayer.Models.Order
{
    public class Order:BaseEntity<Guid>
    {
        public string UserEmail { get; set; } = string.Empty;
        public ShippingAddress ShippingAddress { get; set; } = default!;
        public ICollection<OrderItem> OrderItems { get; set; } = default!;
        public OrderPaymentStatus orderPaymentStatus { get; set; } = OrderPaymentStatus.Pending;
        public OrderStatus orderStatus { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; } = default!;
        
        public int? DeliveryMethodId { get; set; }
        public decimal Subtotal { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public string  PaymentIntentId { get; set; }=string.Empty;
        public decimal GetTotal() => Subtotal + (DeliveryMethod?.Price ?? 0);

    }
}
