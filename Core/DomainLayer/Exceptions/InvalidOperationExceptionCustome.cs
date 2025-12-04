using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions
{
    public sealed class InvalidOperationExceptionCustome : Exception
    {
        public List<string> Errors { get; }

        public InvalidOperationExceptionCustome(List<string> errors)
            : base(string.Join("; ", errors)) // الرسائل تتحول لstring
        {
            Errors = errors;
        }
    }
}
