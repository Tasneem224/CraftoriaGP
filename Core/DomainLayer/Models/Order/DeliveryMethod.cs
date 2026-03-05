using DomainLayer.Models.Items;

namespace DomainLayer.Models.Order
{
    public class DeliveryMethod:BaseEntity<Guid>
    {
        public int Id { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}