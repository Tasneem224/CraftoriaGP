using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.IdentityModule
{
    public class GoogleLoginDto
    {
        public string IdToken { get; set; }
        public RoleType? Role { get; set; }
    }
}
