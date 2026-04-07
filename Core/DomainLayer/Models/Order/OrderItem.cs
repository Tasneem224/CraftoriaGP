using DomainLayer.Models.Items;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models.Order
{

        public class OrderItem : BaseEntity<int>
        {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 👈 تأكدي إن دي موجودة
        public ItemInOrderItem Item { get; set; } = default!;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    
}