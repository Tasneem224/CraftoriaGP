using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Admin_Panel
{
    public class UpdateAdminCategoryDto
    {
        public int Id { get; set; }
        public string? NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; } = string.Empty;
        public IFormFile? image { get; set; }
        public bool? IsVisible { get; set; }
    }
}
