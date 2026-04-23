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
        public IAccountService accountService { get; }
        public IOrderService orderService { get; }
        public IProductService ProductService { get; }
        public ITopRatedService TopRatedService{ get; }
        public IAdminPanelService AdminPanelService { get; }
         public ICategoryService CategoryService { get; }
    }
}
