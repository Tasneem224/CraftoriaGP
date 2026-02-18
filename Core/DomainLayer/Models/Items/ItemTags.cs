using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Items
{
    [PrimaryKey(nameof(ItemId), nameof(TagId))]
    public class ItemTags : BaseEntity<int>
    {
        public int ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = default!;
        public int TagId { get; set; }
        [ForeignKey(nameof(TagId))]
        public Tag Tag { get; set; } = default!;
    }
}
