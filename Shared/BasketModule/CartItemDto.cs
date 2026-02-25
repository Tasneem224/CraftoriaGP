using System.ComponentModel.DataAnnotations;

namespace Shared.BasketModule
{
    public record CartItemDto
    {
        public int Id { get; init; }
        public string UrlImage { get; init; }
        public string ItemName { get; init; }= string.Empty;
        public string CategoryName {  get; init; }= string.Empty;
        public decimal Price {  get; init; }
        public int CategoryId { get; init; }
        [Range(1,50)]
        public int quantity { get; init; }
    }
}