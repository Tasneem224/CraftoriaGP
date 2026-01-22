using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
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
    public class ServiceManager(UserManager<ApplicationUser> _userManager, IConfiguration _configuration, ICloudinaryService _cloudinaryService, IEmailService _emailService,IEmailVerificationCodeRepository _emailVerificationrRepo) : IServiceManager
    {
        private readonly Lazy<IAuthenticationService> _LazyAuthenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(_userManager, _configuration, _cloudinaryService,_emailService, _emailVerificationrRepo));

        public IAuthenticationService AuthenticationService => _LazyAuthenticationService.Value;

    }
}
