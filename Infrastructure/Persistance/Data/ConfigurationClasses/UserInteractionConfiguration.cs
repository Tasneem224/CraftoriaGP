using DomainLayer.Models.Interaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Data.ConfigurationClasses
{
    internal class UserInteractionConfiguration : IEntityTypeConfiguration<UserInteraction>
    {
        public void Configure(EntityTypeBuilder<UserInteraction> builder)
        {
            builder.HasIndex(u => new { u.UserId, u.ProductId })
                   .IsUnique()
                   .HasFilter("[ProductId] IS NOT NULL");



           builder.HasIndex(u => new { u.UserId, u.RawMaterialId })
                   .IsUnique()
                   .HasFilter("[RawMaterialId] IS NOT NULL");

            builder.HasIndex(u => new { u.UserId, u.TargetUserId })
                      .IsUnique()
                      .HasFilter("[TargetUserId] IS NOT NULL");
            builder.HasOne(u => u.Product)
                   .WithMany(p => p.Interactions)
                   .HasForeignKey(u => u.ProductId);

            builder.HasOne(u => u.RawMaterial)
                   .WithMany(r => r.Interactions)
                   .HasForeignKey(u => u.RawMaterialId);



        }
    }
}
