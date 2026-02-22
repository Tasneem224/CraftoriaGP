using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Favourites
{
    public class FavouriteItemDto
    {
        public int Id { get; set; }           // Product Id
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string category { get; set; }
    }
}
