using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interaction
{
    public class InteractionResultDto
    {
        public string USerId { get; set; } = default!;
        public string ItemOrUserId { get; set; } = default!;
        public string NameUser { get; set; } = default!;
        public string NameProduct { get; set; } = default!;
        public string Review { get; set; } = default!;
        public short? Rating { get; set; }
    }
}
