using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProductModule
{
    public class CreateProductDto
    {
        [Required]
        public string NameEn{ get; set; }
        [Required]

        public string NameAr { get; set; }

        [Required]
        public decimal Price { get; set; }
        [Required]


        public int? Quantity { get; set; }
        [Required]

        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [Required]

        public IFormFile? ImageFile { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
