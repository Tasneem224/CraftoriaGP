using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CurrentUserDto
{
    public class UserProfileDto
    {
        public string Id { get; set; }

        public string FirstName { get; set; } = default!;
        public string SecondName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;

        public string? ProfileImage { get; set; } = default!;
        public string? Bio { get; set; } = default!;
        public string? Specialization { get; set; } = default!;
    }
}
