using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
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
        // دي علاقة Many-to-Many
        public string UserId { get; set; }
        public int ProductId { get; set; }

        // Navigation Properties (عشان نعرف نجيب بياناتهم)
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
