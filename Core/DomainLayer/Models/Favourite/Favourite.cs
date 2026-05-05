using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using DomainLayer.Models.RawMaterials;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Favourite
{
    public class Favourite : BaseEntity<int>
    {
        public string UserId { get; set; }
        public int? ProductId { get; set; }
        public int? RawMaterialId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey(nameof(RawMaterialId))]
        public virtual RawMaterial RawMaterial { get; set; }
    }
}
