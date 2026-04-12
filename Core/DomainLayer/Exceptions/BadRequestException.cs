using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions
{
    public sealed class BadRequestException(string message="error"):Exception
    { 
        public BadRequestException():this("error")
        { 

        }
    }
}
