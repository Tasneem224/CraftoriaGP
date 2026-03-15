using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Session
{
    public class AddExpertServiceDto
    {
        // 1. ده اللي الخبير بيبعته وهو بيكريت الخدمة (بيدخل اللغتين)

        [Required(ErrorMessage = "الاسم بالعربي مطلوب")]
        public string TitleAr { get; set; }
        [Required(ErrorMessage = "الاسم بالإنجليزي مطلوب")]
        public string TitleEn { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int DurationInMinutes { get; set; }
        public string? DescriptionAr { get; set; } // ✅ جديد
        public string? DescriptionEn { get; set; } // ✅ جديد


    }
}
