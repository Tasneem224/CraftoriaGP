using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Session
{
    // 4. اليوزر بيحجز
    public class BookSessionDto
    {
        [Required]
        public string ExpertId { get; set; }
        [Required]
        public int ExpertServiceId { get; set; }
        [Required]
        public int ExpertAvailabilityId { get; set; }
    }
}
