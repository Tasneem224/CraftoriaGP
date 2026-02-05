using DomainLayer.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Interaction
{
    public class UserInteraction
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public int TargetId { get; set; }
        public InteractionTargetType TargetType { get; set; }

        public bool? IsFavourite { get; set; }

        [Range(1, 5)]
        public short? Rating { get; set; }

        public string? Review { get; set; }

        public DateTime InteractionDate { get; set; } = DateTime.UtcNow;
    }
}
