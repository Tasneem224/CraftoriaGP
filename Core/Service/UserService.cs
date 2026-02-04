using DomainLayer.Exceptions;
using DomainLayer.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction;
using Shared.IdentityModule;
using Shared.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICloudinaryService _cloudinaryService;


        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, ICloudinaryService cloudinaryService)
        {


            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;

        }
        public async Task<UserProfileDto> GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new UserNotFoundException("User not found");

            // ⭐ جلب roles
            var roles = await _userManager.GetRolesAsync(user);

            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                UserName = user.UserName,
                Email = user.Email,
                ProfileImage = user.ProfileImage,
                Bio = user.Bio,
                Specialization = user.Specialization,

                // ⭐ تحويل role إلى enum
                roleType = roles.Any()
                    ? Enum.Parse<RoleType>(roles.First())
                    : RoleType.Customer
            };
        }

        public async Task<UserProfileDto> UpdateUserProfile(UpdateUserDto updateUserDto)
        {
            var userId = _httpContextAccessor.HttpContext?
               .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new UserNotFoundException("User not found");

            checkUpdateData(updateUserDto, user);

            if (updateUserDto.ProfileImage != null)
            {
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    _cloudinaryService.DeleteAsync(user.ProfileImage);
                }
                var imageUrl = await _cloudinaryService.UploadAsync(updateUserDto.ProfileImage);
                user.ProfileImage = imageUrl;
            }
            await _userManager.UpdateAsync(user);


            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                UserName = user.UserName,
                Email = user.Email,
                ProfileImage = user.ProfileImage,
                Bio = user.Bio,
                Specialization = user.Specialization
            };
        }

        private static void checkUpdateData(UpdateUserDto updateUserDto, ApplicationUser user)
        {
            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
                user.FirstName = updateUserDto.FirstName;

            if (!string.IsNullOrEmpty(updateUserDto.LastName))
                user.SecondName = updateUserDto.LastName;

            if (!string.IsNullOrEmpty(updateUserDto.Bio))
                user.Bio = updateUserDto.Bio;

            if (!string.IsNullOrEmpty(updateUserDto.Specialization))
                user.Specialization = updateUserDto.Specialization;

            if (updateUserDto.Gender.HasValue)
                user.Gender = (Gender)updateUserDto.Gender.Value;
        }
    }
}
