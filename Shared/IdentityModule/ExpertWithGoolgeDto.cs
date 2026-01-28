using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.IdentityModule
{
    public class ExpertWithGoolgeDto
    {
       public int YearsOfExp { get; set; }
       public string Email { get; set; }
        public IFormFile portfolio { get; set; }
    }
}
