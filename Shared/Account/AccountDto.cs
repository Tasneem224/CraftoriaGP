using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Account
{
    public class ReturnAccountDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; }= string.Empty;
        public string SecondName { get; set; }= string.Empty;
        public string Email { get; set; }= string.Empty;
        public string ProfileImage { get; set; }=string.Empty;
        public string Role { get; set; }= string.Empty;
        public string Bio { get; set; }=string.Empty;
        public int YearOfExperience { get; set; }
        public string Specialization { get; set; }= string.Empty;

    }
}
