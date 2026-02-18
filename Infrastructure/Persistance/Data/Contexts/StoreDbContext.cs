using DomainLayer.Models;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Favourite;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Interaction;
using DomainLayer.Models.Items;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.ConfigurationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Data.Contexts
{
    public class StoreDbContext(DbContextOptions<StoreDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserInteraction> UserInteractions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Raw_Category_Material> RawMaterialCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Ignore<IdentityUserClaim<string>>();
            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityRoleClaim<string>>();
            
            builder.Entity<Favourite>()
                .HasIndex(f => new { f.UserId, f.ProductId })
                .IsUnique();

            builder.Entity<Favourite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Item>().UseTpcMappingStrategy();
            builder.ApplyConfigurationsFromAssembly(typeof(ReferenceAssembly).Assembly);

        }

 
        public DbSet<EmailVerificationCodes> EmailVerificationCodes { get; set; }

    }
}
