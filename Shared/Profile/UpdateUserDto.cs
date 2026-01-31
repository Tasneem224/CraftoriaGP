using Microsoft.AspNetCore.Http;
using Shared.IdentityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Profile
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; } = default!;
        public string? LastName { get; set; } = default!;
        public string? Bio { get; set; } = default!;
        public Gender2? Gender { get; set; } = default!;
        public string? Specialization { get; set; } = default!;

        public IFormFile? ProfileImage { get; set; }
    }
}
