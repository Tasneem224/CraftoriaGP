using DomainLayer.Models.Items;

namespace DomainLayer.Models.Order
{

        public class OrderItem : BaseEntity<int>
        {
            public ItemInOrderItem Item { get; set; } = default!;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    
}