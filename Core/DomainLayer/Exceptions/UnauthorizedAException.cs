using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions
{
    using System;

    namespace DomainLayer.Exceptions
    {
        public sealed class UnauthorizedAException : Exception
        {
           
            public UnauthorizedAException() : base("invalid email or password")
            {
            }
            public UnauthorizedAException(string message)
       : base(message) 
            {
            }


        }
    }

}
