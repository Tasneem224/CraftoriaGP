using DomainLayer.Models.Items;
using DomainLayer.Models.RawMaterials;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Categories
{
    public class Raw_Category_Material:BaseEntity<int>
    {
        
        public string Name { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;


        [InverseProperty(nameof(RawMaterial.Category))]
        public ICollection<RawMaterial>? rawMaterials { get; set; }

    }
}
