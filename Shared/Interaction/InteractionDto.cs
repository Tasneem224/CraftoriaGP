using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interaction
{

    public class InteractionDto<T>
    {
        public string UserId { get; set; } = default!;
        public T ItemOrUserId { get; set; } = default!;
        public short? Range { get; set; }
        public string? Review { get; set; }
    }
}
