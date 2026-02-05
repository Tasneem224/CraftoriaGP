using DomainLayer.Models.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interaction
{

    public class InteractionDto
    {
        public string UserId { get; set; } = default!;
        public string TargetId { get; set; } = default!;
        public InteractionTargetType2 TargetType { get; set; }
        public short? Rating { get; set; }
        public string? Review { get; set; }
        public bool? IsFavourite { get; set; }
    }
}
