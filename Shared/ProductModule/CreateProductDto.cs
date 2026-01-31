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
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int? Quantity { get; set; }
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public string SellerId {  get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
