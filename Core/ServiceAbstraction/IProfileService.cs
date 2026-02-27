using Shared.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IProfileService
    {

        Task<UserProfileDto> GetCurrentUserAsync();
        Task<UserProfileDto> UpdateUserProfile(UpdateUserDto updateUserDto);
        Task<IEnumerable<ReviewsProfile>> GetAllReviewsCreatedByUser(string userId);
    }
}
