using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Profile;
using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [Authorize]
    public class UserProfileController(IUserService _userService, IStringLocalizer<SharedResources> _stringLocalizer) : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userService.GetCurrentUserAsync();
            return SendSuccessResponse(user, _stringLocalizer[SharedResourcesKeys.CurrentUserDataReturnedSuccessfully]);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUserProfile(updateUserDto);
            return SendSuccessResponse(updatedUser, _stringLocalizer[SharedResourcesKeys.UserProfileUpdatedSuccessfully]);
        }

    }
}
