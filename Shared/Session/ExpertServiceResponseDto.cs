using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Session
{
    public class ExpertServiceResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } // هيشيل عربي أو إنجليزي
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
        public string? Description { get; set; } // ✅ جديد

    }
}
