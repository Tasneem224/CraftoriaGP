using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions
{
    public sealed class UserNotFoundException (string Email) :NotFoundException($"there is nobody with this email {Email}")
    {
      
    }
}
