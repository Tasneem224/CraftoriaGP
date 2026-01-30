using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Categories
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;

        [InverseProperty(nameof(Product.category))]
        public  ICollection<Product>? products { get; set; }
    }
}
