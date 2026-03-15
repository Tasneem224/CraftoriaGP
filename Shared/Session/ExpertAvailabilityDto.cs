using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Session
{
    
        public class ExpertAvailabilityDto
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan StartTime { get; set; }
         
            public bool IsAvailable { get; set; }
        
    }
}
