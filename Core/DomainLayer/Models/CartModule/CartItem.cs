using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.CartModule
{
    public class CartItem
    {
        public int Id { get; set; }
        public string ItemNameEn { get; set; }
        public string ItemNameAr { get; set; }
        public string PictureURL { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; init; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
