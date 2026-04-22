using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Admin_Panel
{
    public class ReturnAdminCategoriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
    }
}
