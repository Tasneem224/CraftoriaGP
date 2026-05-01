using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Admin_Panel
{
    public class RegisterAdmintDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = default!;
        [MaxLength(50)]
        [Required]
        public string SecondName { get; set; } = default!;
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Required]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = default!;
        public IFormFile Image { get; set; }


    }
}
