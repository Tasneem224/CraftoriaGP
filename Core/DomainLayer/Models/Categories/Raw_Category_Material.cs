using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Categories
{
    public class Raw_Category_Material:BaseEntity<int>
    {
        
        public string Name { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;

    }
}
