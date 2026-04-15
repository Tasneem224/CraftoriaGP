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
        public sealed class UnauthorizedException : Exception
        {
           
            public UnauthorizedException() : base("invalid email or password")
            {
            }
            public UnauthorizedException(string message)
       : base(message) 
            {
            }


        }
    }

}
