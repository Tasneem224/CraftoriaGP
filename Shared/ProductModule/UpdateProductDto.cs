using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProductModule
{
    public class UpdateProductDto : CreateProductDto
    {
        // بنخلي الصورة اختيارية في التعديل
        public new IFormFile? ImageFile { get; set; }
    }
}
