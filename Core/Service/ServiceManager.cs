using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceManager(IHttpContextAccessor _httpContextAccessor,ITranslationService _translationService,ICartRepository cartRepository,ICartService cartService,IAccountService accountService,ICacheRepository cacheRepository,ICacheService cacheService,IUnitOfWork unitOfWork,UserManager<ApplicationUser> _userManager, IConfiguration _configuration, ICloudinaryService _cloudinaryService, IEmailService _emailService,IEmailVerificationCodeRepository _emailVerificationrRepo,IMapper _mapper,ICartRepository _cacheRepository) : IServiceManager
    {
        private readonly Lazy<IAuthenticationService> _LazyAuthenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(_userManager, _configuration, _cloudinaryService,_emailService, _emailVerificationrRepo));
        private readonly Lazy<ICartService> _LazyCartService = new Lazy<ICartService>(() => new CartService(unitOfWork, _cacheRepository,_mapper));
        private readonly Lazy<ICacheService> _LazyCacheService = new Lazy<ICacheService>(() => new CacheService(cacheRepository));
        private readonly Lazy<IAccountService> _LazyAccountService = new Lazy<IAccountService>(() => new AccountService(_userManager, unitOfWork));
        private readonly Lazy<IOrderService> _LazyOrderService = new Lazy<IOrderService>(() => new OrderService(_userManager, cartRepository, unitOfWork,cartService));
        private readonly Lazy<IProductService> _LazyProductService = new Lazy<IProductService>(() => new ProductService(_translationService, _httpContextAccessor, _userManager,_cloudinaryService,unitOfWork));
        private readonly Lazy<ITopRatedService> _LazyTopRatedService = new Lazy<ITopRatedService>(() => new TopRatedService(unitOfWork,_userManager));
        

        public IProductService ProductService => _LazyProductService.Value;
        public IOrderService orderService => _LazyOrderService.Value;
        public IAccountService accountService => _LazyAccountService.Value;
        public ICacheService cacheService => _LazyCacheService.Value;
        public IAuthenticationService AuthenticationService => _LazyAuthenticationService.Value;
        public ICartService CartService => _LazyCartService.Value;
        public ITopRatedService TopRatedService => _LazyTopRatedService.Value;
    }
}
