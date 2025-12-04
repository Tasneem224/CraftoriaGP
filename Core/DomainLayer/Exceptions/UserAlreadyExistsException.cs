using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions
{
    public sealed class UserAlreadyExistsException(string email):NotFoundException("this email is Already exist try another one")
    {
    }
}
