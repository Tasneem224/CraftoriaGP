using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using DomainLayer.Models.RawMaterials;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Interaction
{
    public class UserInteraction:BaseEntity<int>
    {

        public string UserId { get; set; } = default!;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = default!;

        public int? ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }


        public int? RawMaterialId { get; set; }
        [ForeignKey(nameof(RawMaterialId))]
        public RawMaterial? RawMaterial { get; set; }

        public string? TargetUserId { get; set; } 

        [ForeignKey(nameof(TargetUserId))]
        public ApplicationUser? TargetUser { get; set; }

        public bool? IsFavourite { get; set; }

        [Range(1, 5)]
        public short? Rating { get; set; }

        public string? Review { get; set; }

        public DateTime InteractionDate { get; set; } = DateTime.UtcNow;
    }
}
