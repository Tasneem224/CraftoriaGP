using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Categories
{
    [Table("ProductCategories")]

    public class ProductCategory:BaseEntity<int>
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;

        [InverseProperty(nameof(Product.Category))]
        public  ICollection<Product>? products { get; set; }
    }
}
