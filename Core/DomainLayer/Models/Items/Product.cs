using DomainLayer.Models.Categories;
using DomainLayer.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Items
{
    [Table("Products")]
    public class Product:Item
    {
        [DisplayName("Seller")]
        [ForeignKey("Seller")]
        public string SellerId { get; set; } = default!;
        public ApplicationUser Seller { get; set; } = default!;


        // [DisplayName("Category")]
        // [ForeignKey("Category")]
        //// public int CategoryId {  get; set; }
        //// public Category Category { get; set; }
        ///
        [DisplayName("Category")]
        [ForeignKey("Category")]
         public int CategoryId {  get; set; }

        [InverseProperty(nameof(Category.products))]
        public Category category { get; set; } = default!;




    }
}
