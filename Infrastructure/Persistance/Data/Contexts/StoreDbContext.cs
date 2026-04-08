using DomainLayer.Models;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Favourite;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Interaction;
using DomainLayer.Models.Items;
using DomainLayer.Models.Messages;
using DomainLayer.Models.Notifications;
using DomainLayer.Models.Order;
using DomainLayer.Models.RawMaterials;
using DomainLayer.Models.session;
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
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<Address_Book> Adress_Shipping { get; set; }
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
            builder.Ignore<IdentityUserClaim<string>>();
            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityRoleClaim<string>>();

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
            builder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);
            // 5. الـ Favourite Configuration (يفضل تنقليها لكلاس منفصل لاحقاً بس شغالة هنا)
            builder.Entity<Favourite>()
                .HasIndex(f => new { f.UserId, f.ProductId })
                .IsUnique();

            builder.Entity<Favourite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 🚨 ملاحظة: تم حذف السطر المكرر لـ ApplyConfigurations و ReferenceAssembly

            // تظبيط علاقات الرسائل عشان نمنع الـ Cascade Delete
            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }


        public DbSet<EmailVerificationCodes> EmailVerificationCodes { get; set; }

    }
}
