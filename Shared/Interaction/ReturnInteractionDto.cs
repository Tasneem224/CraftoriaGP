using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interaction
{
    public class ReturnInteractionDto<T>
    {
        public string USerId { get; set; } = default!;
        public T ItemOrUserId { get; set; } = default!;
        public string NameUser { get; set; } = default!;
        public string NameProduct { get; set; } = default!;
        public string Review { get; set; } = default!;
        public short Range { get; set; }
    }
}
