using DomainLayer.Models.Interaction;
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
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;

        [Range(0, int.MaxValue)]
        public int? Quantity {  get; set; } = 0;

        [Range(0, float.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price {  get; set; }  =default!;
        public string? DescriptionEn { get; set; } = default!;
        public string? DescriptionAr { get; set; } = default!;
    
        public virtual ICollection<Tag> tags { get; set; } = new HashSet<Tag>();

    }
}
