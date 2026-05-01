using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CommunityModule
{
    public class PostCreateDto
    {
        public string Content { get; set; }
        public IFormFile? Image { get; set; } // لاستقبال الصورة من الموبايل
    }
}
