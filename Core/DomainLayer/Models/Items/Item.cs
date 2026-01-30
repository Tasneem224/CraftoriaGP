using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Items
{
    public abstract class Item:BaseEntity<int>
    {
        public string Name { get; set; } = default!;
        public string? ImageUrl { get; set; } = default!;

        [Range(0, int.MaxValue)]
        public int? Quantity {  get; set; } = 0;

        [Range(0, float.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price {  get; set; }  =default!;
        public string? Description { get; set; } = default!;
        


        
    }
}
