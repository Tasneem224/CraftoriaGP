using System.ComponentModel.DataAnnotations;

namespace Shared.BasketModule
{
    public record CartItemDto
    {
        public int Id { get; init; }
        public string PictureURL { get; set; } = default!;
        public string ItemName { get; set; }= string.Empty;
       
        public string Category {  get; set; }= string.Empty;
        public decimal Price {  get; set; }
        public int CategoryId { get; set; }
        public int Quantity { get; init; }
    }
}