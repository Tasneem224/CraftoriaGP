using Shared.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IUserService
    {
        Task<UserProfileDto> GetCurrentUserAsync();
        Task<UserProfileDto> UpdateUserProfile(UpdateUserDto updateUserDto);
    }
}
