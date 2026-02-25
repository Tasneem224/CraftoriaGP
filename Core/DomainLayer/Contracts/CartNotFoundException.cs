using DomainLayer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public sealed class CartNotFoundException(string id):NotFoundException($"cart  with id {id} is not found")
    {
    }
}
