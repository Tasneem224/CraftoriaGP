using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.IdentityModule
{
    public class RegisterDto
    {
        //[Required(ErrorMessage = "First name is required")]
        //[StringLength(50, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = default!;

        [Required(ErrorMessage = "Last name is required")]


        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 50 characters")]
        public string LastName { get; set; } = String.Empty!;

        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;        
        public string ConfirmPassword { get; set; } = default!;

        [Required]
        public RoleType Role { get; set; }
        [Required]
        public Gender2 Gender { get; set; }
        public int? YearsOfExperience { get; set; } = default;
        public IFormFile? ProfileImage { get; set; }
        public IFormFile? Portfolio { get; set; } = default;
    }
}
