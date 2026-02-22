using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using ServiceAbstraction;
using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Resources;

namespace Service
{
    public class ServiceManager(UserManager<ApplicationUser> _userManager, IConfiguration _configuration, ICloudinaryService _cloudinaryService, IEmailService _emailService,IEmailVerificationCodeRepository _emailVerificationrRepo, IStringLocalizer<SharedResources> _stringLocalizer) : IServiceManager
    {
        private readonly Lazy<IAuthenticationService> _LazyAuthenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(_userManager, _configuration, _cloudinaryService,_emailService, _emailVerificationrRepo,_stringLocalizer));

        public IAuthenticationService AuthenticationService => _LazyAuthenticationService.Value;

    }
}
