using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Order;
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
    public class DataSeeding(ICartRepository cartRepository,StoreDbContext context,UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager) : IDataSeeding
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
        public  async Task SeedCustomersDataAsync(  )
        {
            if (!await context.DeliveryMethods.AnyAsync())
            {
                var methods = new List<DeliveryMethod>
        {
            new DeliveryMethod { ShortName = "Standard", Description = "3-5 Days", Price = 30m, DeliveryTime = "5 Days" },
            new DeliveryMethod { ShortName = "Express", Description = "1-2 Days", Price = 70m, DeliveryTime = "2 Days" }
        };
                context.DeliveryMethods.AddRange(methods);
                await context.SaveChangesAsync();
            }
            var random = new Random();

            var allUsers = await _userManager.Users.ToListAsync();
            var customers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    customers.Add(user);
                }
            }

            if (!customers.Any()) return;

            var products = await context.Products.Take(50).ToListAsync();
            var deliveryMethods = await context.DeliveryMethods.ToListAsync();

            foreach (var customer in customers)
            {
                if (!context.AddressBooks.Any(a => a.AppUserId == customer.Id))
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        context.AddressBooks.Add(new Address_Book
                        {
                            Id = Guid.NewGuid(),
                            AppUserId = customer.Id,
                            FullName = $"{customer.FirstName} {customer.SecondName} - Address {i}",
                            City = i == 1 ? "Cairo" : (i == 2 ? "Alexandria" : "Giza"),
                            State = "Egypt",
                            StreetDetails = $"Street {random.Next(1, 100)} Building {i}",
                            PhoneNumber = customer.PhoneNumber ?? "01" + random.Next(100000000, 999999999),
                            Region = "District " + i
                        });
                    }
                }

                
                if (!context.Orders.Any(o => o.UserEmail == customer.Email))
                {
                    for (int k = 1; k <= 2; k++)
                    {
                        var randomDelivery = deliveryMethods[random.Next(deliveryMethods.Count)];
                        var orderItems = new List<OrderItem>();
                        decimal subtotal = 0;

                        // كل أوردر فيه منتجين عشوائيين
                        for (int j = 0; j < 2; j++)
                        {
                            var prod = products[random.Next(products.Count)];
                            subtotal += prod.Price;
                            orderItems.Add(new OrderItem
                            {
                                Item = new ItemInOrderItem { ItemId = prod.Id, ItemName = prod.NameEn, ItemPictureUrl = prod.ImageUrl ?? "" },
                                Price = prod.Price,
                                Quantity = 1
                            });
                        }

                        context.Orders.Add(new Order
                        {
                            Id = Guid.NewGuid(),
                            UserEmail = customer.Email,
                            OrderDate = DateTimeOffset.UtcNow.AddDays(-random.Next(1, 30)),
                            DeliveryMethodId = randomDelivery.Id,
                            Subtotal = subtotal,
                            orderStatus = OrderStatus.Pending,
                            orderPaymentStatus = OrderPaymentStatus.Pending,
                            ShippingAddress = new Address
                            {
                                FullName = customer.DisplayName,
                                City = "Cairo",
                                StreetDetails = "Default Street",
                                PhoneNumber = customer.PhoneNumber ?? "010000000",
                                Region = "Main District"
                            },
                            OrderItems = orderItems
                        });
                    }
                }
            }

            await context.SaveChangesAsync();
        }
      
    }
}
    
