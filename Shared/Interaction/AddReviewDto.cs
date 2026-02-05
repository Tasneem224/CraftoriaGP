using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interaction
{
    public class AddReviewDto
    {
        public int? ProductId { get; set; }
        public int? RawMaterialId { get; set; }
        public string? TargetUserId { get; set; }

        [Range(1, 5)]
        public short Rating { get; set; }
        public string? Review { get; set; }
    }
}
