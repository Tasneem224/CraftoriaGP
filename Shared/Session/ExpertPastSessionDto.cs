using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Session
{
    public class ExpertPastSessionDto
    {
        public int SessionId { get; set; }
        public string BeginnerName { get; set; }
        public string BeginnerImageUrl { get; set; } = string.Empty;
        public string ServiceName { get; set; }
        public DateTime Date { get; set; }
        public string Duration { get; set; }      // "3:00 PM • 60 min"
        public string Status { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
