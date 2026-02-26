using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IServiceManager
    {
        public IAuthenticationService AuthenticationService { get; }
        public ICartService CartService { get; }
        public ICacheService cacheService { get; }

    }
}
