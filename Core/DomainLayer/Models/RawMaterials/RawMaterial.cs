using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.RawMaterials
{
    public class RawMaterial:Item
    {
        public string supplierId { get; set; } = default!;

        [ForeignKey(nameof(supplierId))]
        public ApplicationUser supplier { get; set; } = default!;

    }
}
