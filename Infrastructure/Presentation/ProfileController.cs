using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [Authorize]
    public class UserProfileController(IProfileService _userService) : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userService.GetCurrentUserAsync();
            return SendSuccessResponse(user, "Data of the current User returned successfully");

        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUserProfile(updateUserDto);
            return SendSuccessResponse(updatedUser, "User Profile updated successfully");
        }
        [HttpGet("GetAllReviewThatCreatedBySpecificUser")]
        [Authorize] 
        public async Task<IActionResult> GetAllReviewThatCreatedBySpecificUser()
        {
            var reviews = await _userService.GetAllReviewsCreatedByUser();
            return SendSuccessResponse(reviews);
        }
    }
}
