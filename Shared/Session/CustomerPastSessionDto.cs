using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Session
{
    public class CustomerPastSessionDto
    {
        public int SessionId { get; set; }
        public string ExpertId { get; set; }
        public string ExpertName { get; set; }
        public string ExpertImageUrl { get; set; }
        public string ServiceName { get; set; }
        public DateTime Date { get; set; }
        public string TimeAndDuration { get; set; }   // "3:00 PM • 60 min"
        public string Status { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
