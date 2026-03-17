using Bogus;
using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Interaction;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class DataSeeding(StoreDbContext _context,UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager) : IDataSeeding
    {
        public async Task IdentityDataSeedingAsync()
        {

            try
            {
                if (_roleManager.Roles.Any() is false)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    await _roleManager.CreateAsync(new IdentityRole("Supplier"));
                    await _roleManager.CreateAsync(new IdentityRole("Expert"));
                    await _roleManager.CreateAsync(new IdentityRole("Beginner"));
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

        public async Task SeedOneThousandUsers()
        {
          
            //if (_userManager.Users.Any(u => u.IsGenerated)) return;

            //var faker = new Faker<ApplicationUser>()
            //    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            //    .RuleFor(u => u.SecondName, f => f.Name.LastName())
            //    .RuleFor(u => u.DisplayName, (f, u) => u.FirstName + " " + u.SecondName)
            //    .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.SecondName))
            //    .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.SecondName))
            //    .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
            //    .RuleFor(u => u.IsGenerated, true);

            //for (int i = 0; i < 1000; i++)
            //{
            //    var user = faker.Generate();
            //    string roleToAssign;

            //    if (i < 100)
            //    {
            //        user.YearsOfExperience = new Random().Next(5, 15);
            //        user.Specialization = "Handmade Crafts Expert";
            //        roleToAssign = "Expert";
            //    }
            //    else if (i < 300) 
            //    {
            //        user.Bio = "Global supplier of high-quality raw materials.";
            //        roleToAssign = "Supplier";
            //    }
            //    else if (i < 600) 
            //    {
            //        user.Bio = "Beginner artisan looking for materials and learning.";
            //        roleToAssign = "Beginner";
            //    }
            //    else 
            //    {
            //        user.Bio = "Art enthusiast and buyer of handmade products.";
            //        roleToAssign = "Customer";
            //    }

            //    var result = await _userManager.CreateAsync(user, "Password123!");

            //    if (result.Succeeded)
            //    {
                   
            //        await _userManager.AddToRoleAsync(user, roleToAssign);
            //    }
            //}
        }
        public async Task SeedReviews()
        {
            // 1. جلب المستخدمين مع الأدوار في Query واحدة سريعة جداً
            // بنستخدم Join بين المستخدمين وجدول الأدوار
            var usersWithRoles = await _userManager.Users
                .Where(u => u.IsGenerated)
                .Select(u => new
                {
                    User = u,
                    RoleName = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // 2. تقسيمهم في الميموري (مفيش روح داتا بيز هنا خلاص)
            var experts = usersWithRoles.Where(x => x.RoleName == "Expert").Select(x => x.User).ToList();
            var beginners = usersWithRoles.Where(x => x.RoleName == "Beginner").Select(x => x.User).ToList();
            var customers = usersWithRoles.Where(x => x.RoleName == "Customer").Select(x => x.User).ToList();

            var random = new Random();
            var reviews = new List<UserInteraction>();
            var reviewFaker = new Faker();

            // جمل تقييم واقعية
            var reviewTexts = new[] { "Amazing!", "Great work", "Professional", "Very helpful", "Excellent skills" };

            // 3. توليد الـ 100 تقييم (نفس الـ Logic بتاعك)
            for (int i = 0; i < 100; i++)
            {
                ApplicationUser reviewer = null;
                ApplicationUser target = null;

                if (i < 50 && beginners.Any() && experts.Any())
                {
                    reviewer = beginners[random.Next(beginners.Count)];
                    target = experts[random.Next(experts.Count)];
                }
                else if (customers.Any())
                {
                    reviewer = customers[random.Next(customers.Count)];
                    var pool = (random.Next(2) == 0 && experts.Any()) ? experts : beginners;
                    if (pool.Any()) target = pool[random.Next(pool.Count)];
                }

                if (reviewer != null && target != null)
                {
                    reviews.Add(new UserInteraction
                    {
                        UserId = reviewer.Id,
                        TargetUserId = target.Id,
                        Rating = (short)random.Next(3, 6),
                        Review = reviewFaker.PickRandom(reviewTexts),
                        CreatedAt = DateTime.Now.AddDays(-random.Next(1, 20))
                    });
                }
            }

            // 4. حفظ الكل مرة واحدة
            if (reviews.Any())
            {
                _context.UserInteractions.AddRange(reviews);
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ Done! 100 reviews seeded successfully.");
            }
        }
    }
}
    
