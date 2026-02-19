using DomainLayer.Models.RawMaterials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Items
{
    public class Tag:BaseEntity<int>
    {
        public string Name { get; set; } =default!;
        public virtual ICollection<Item> items { get; set; } = new HashSet<Item>();
    }
}
