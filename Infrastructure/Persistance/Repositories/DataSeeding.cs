using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class DataSeeding(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager) : IDataSeeding
    {
        public async Task IdentityDataSeedingAsync()
        {

            try
            {
                if (_roleManager.Roles.Any() is false)
                {
                    //await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    //await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    //await _roleManager.CreateAsync(new IdentityRole("Supplier"));
                    //await _roleManager.CreateAsync(new IdentityRole("Expert"));
                    //await _roleManager.CreateAsync(new IdentityRole("Beginner"));
                }
                if (_userManager.Users.Any() is false)
                {
                    //tasneem
                    //var user1 = new ApplicationUser
                    //{
                    //    UserName = "Tasneemtaha224",
                    //    Email = "tasneemtaha224@gmail.com",
                    //    DisplayName = "Tasneem Taha",
                    //    Gender = Gender.Female,
                    //    FirstName = "Tasneem",
                    //    SecondName = "Taha",
                    //    PhoneNumber = "01091341187",

                    //};
                    //afnan
                    //var user2 = new ApplicationUser
                    //{
                    //    UserName = "AfnanAli",
                    //    Email = "Afnan2@gmail.com",
                    //    DisplayName = "Afnan Ali",
                    //    Gender = Gender.Female,
                    //    FirstName = "Afnan",
                    //    SecondName = "Ali",
                    //    PhoneNumber = "01099807400",

                    //};
                    ////shiamaa
                    //var user3 = new ApplicationUser
                    //{
                    //    UserName = "Shiamaa22",
                    //    Email = "shiamaa925@gmail.com",
                    //    DisplayName = "shiamaa Taha",
                    //    Gender = Gender.Female,
                    //    FirstName = "shiamaa",
                    //    SecondName = "Taha",
                    //    PhoneNumber = "01017830157",

                    //};
                    //mohamed taha
                    //var user4 = new ApplicationUser
                    //{
                    //    UserName = "mohamedtaha3",
                    //    Email = "mohamedtaha22962@gmail.com",
                    //    DisplayName = "Mohamed Taha",
                    //    Gender = Gender.Male,
                    //    FirstName = "Mohamed",
                    //    SecondName = "Taha",
                    //    PhoneNumber = "01024089691",

                    //};
                    //gehad taha
                    //var user5 = new ApplicationUser
                    //{
                    //    UserName = "gehadtaha",
                    //    Email = "ghdtahagenedy@gmail.com",
                    //    DisplayName = "Gehad Taha",
                    //    Gender = Gender.Female,
                    //    FirstName = "Gehad",
                    //    SecondName = "Taha",
                    //    PhoneNumber = "01033391219",

                    //};
                    ////await _userManager.CreateAsync(user1, "P@ssw0rd");
                    //await _userManager.CreateAsync(user2, "P@ssw0rd");
                    //await _userManager.CreateAsync(user3, "P@ssw0rd");
                    ////await _userManager.CreateAsync(user4, "P@ssw0rd");
                    //await _userManager.CreateAsync(user5, "P@ssw0rd");

                    ////await _userManager.AddToRoleAsync(user1, "Admin");
                    //await _userManager.AddToRoleAsync(user2, "Customer");
                    //await _userManager.AddToRoleAsync(user3, "Supplier");
                    ////await _userManager.AddToRoleAsync(user4, "Expert");
                    //await _userManager.AddToRoleAsync(user5, "Admin");

                }


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
    
