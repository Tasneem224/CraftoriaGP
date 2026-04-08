using DomainLayer.Models;
using DomainLayer.Models.Categories;
using DomainLayer.Models.ChatBot;
using DomainLayer.Models.Favourite;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Interaction;
using DomainLayer.Models.Items;
using DomainLayer.Models.Order;
using DomainLayer.Models.RawMaterials;
using DomainLayer.Models.session;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
        public DbSet<ChatBotMessages> ChatBotMessages { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<Address_Book> AddressBooks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem>  OrderItems { get; set; }
        public DbSet<ExpertService> ExpertServices { get; set; }
        public DbSet<ExpertAvailability> ExpertAvailabilities { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserInteraction> UserInteractions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Raw_Category_Material> RawMaterialCategories { get; set; }
        public DbSet<RawMaterial> RawMaterials { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // 1. Identity Tables Configuration
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");

            // تجاهل الجداول الإضافية لـ Identity عشان الـ Warnings اللي كانت بتظهر
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            //builder.Ignore<IdentityUserClaim<string>>();
            //builder.Ignore<IdentityUserToken<string>>();
            //builder.Ignore<IdentityUserLogin<string>>();
            //builder.Ignore<IdentityRoleClaim<string>>();

            // 2. تطبيق الـ Configuration Classes (السطر ده كفاية جداً لكل الملفات اللي في الـ Assembly)
            builder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);

            // 3. ضبط الـ Decimal Properties (عشان نلغي الـ Warnings الصفراء)
            builder.Entity<ApplicationUser>()
                .Property(u => u.CommissionRate)
                .HasColumnType("decimal(18,2)");

            builder.Entity<ExpertService>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Session>()
                .Property(s => s.AmountPaid)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Item>()
    .HasMany(i => i.tags)
    .WithMany(t => t.items)
    .UsingEntity<ItemTags>(
        j => j.HasOne(it => it.Tag)
              .WithMany()
              .HasForeignKey(it => it.TagId),
        j => j.HasOne(it => it.Item)
              .WithMany()
              .HasForeignKey(it => it.ItemId)
    );
            // 4. Inheritance Strategy
            builder.Entity<Item>().UseTpcMappingStrategy();
            // 5. الـ Favourite Configuration (يفضل تنقليها لكلاس منفصل لاحقاً بس شغالة هنا)
            builder.Entity<Favourite>()
                .HasIndex(f => new { f.UserId, f.ProductId })
                .IsUnique();

            builder.Entity<Favourite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 2. Value Converter لكل الـ decimals
            var decimalConverter = new ValueConverter<decimal, decimal>(
                v => v,           // from model to db
                v => v            // from db to model
            );

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));

                foreach (var property in properties)
                {
                    // تحقق إذا العمود معمول له HasColumnType مسبقًا
                    var existingProperty = builder.Entity(entityType.Name).Metadata.FindProperty(property.Name);
                    if (existingProperty.GetColumnType() == null)  // فقط الأعمدة بدون ColumnType مسبق
                    {
                        builder.Entity(entityType.Name)
                               .Property(property.Name)
                               .HasConversion(decimalConverter)
                               .HasColumnType("numeric(18,2)");
                    }
                }
            }
        }


        public DbSet<EmailVerificationCodes> EmailVerificationCodes { get; set; }

    }
}
