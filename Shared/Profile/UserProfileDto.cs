using Shared.IdentityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Profile
{
    public class UserProfileDto
    {
        public string Id { get; set; } = default!;

        public string FirstName { get; set; } = default!;
        public string SecondName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int? YearOfExperience { get; set; } = default!;
        public string? ProfileImage { get; set; } = default!;
        public RoleType roleType { get; set; }
        public string? Bio { get; set; } = default!;
        public string? Specialization { get; set; } = default!;
    }
}
